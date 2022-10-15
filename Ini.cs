using System.Collections;
using System.Text;

namespace Chloride.CCiniExt
{
    public class Ini : IEnumerable<IniSection>
    {
        // when "merging" mutiples into one,
        // diff shouldn't reset as "+%d=" pair may be replaced.
        internal int diff = 0;

        private List<string?> Header = new();
        private List<IniSection> Raw = new();

        public Ini() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // To support GB2312.

        public IniSection this[string sec]
        {
            get => Contains(sec, out IniSection? sect) ? sect! : throw new KeyNotFoundException(sec);
            set
            {
                value.Name = sec; // assignment should keep correspondence.

                if (HasSection(sec, out int idx))
                    Raw[idx].Update(value);
                else
                    Raw.Add(value);
            }
        }

        // the following private methods are just for internal.
        // won't consider index assignment for users.
        private int IndexOf(string section)
        {
            foreach (var i in Raw)
            {
                if (i.Name == section)
                    return Raw.IndexOf(i);
            }
            return -1;
        }
        private bool HasSection(string section, out int index) => (index = IndexOf(section)) != -1;

        public bool Contains(string section, out IniSection? result) => (result = Raw.LastOrDefault(i => i.Name == section)) != null;
        public bool ContainsKey(string section, string key) => HasSection(section, out int idx) && Raw[idx].Contains(key, out _);

        // wouldn't replace the old one.
        public void AddNew(string sect)
        {
            if (!Contains(sect, out _))
                Raw.Add(new(sect));
        }
        public void Remove(string sect)
        {
            if (HasSection(sect, out int idx))
                Raw.RemoveAt(idx);
        }
        public void Rename(string _old, string _new)
        {
            if (Contains(_old, out IniSection? old))
            {
                if (_old == _new || IndexOf(_new) != -1)
                    throw new ArgumentException($"Section {_new} already exists");
                old!.Name = _new;
            }
        }

        public string[] GetTypeList(string sect) => Contains(sect, out IniSection? ret) ? ret!.Values.Select(i => i.ToString()).ToArray() : Array.Empty<string>();
        public void SetValue(string sect, string key, IniValue value)
        {
            AddNew(sect);
            Raw[IndexOf(sect)][key] = value;
        }

        public void Clear() => Raw.Clear();
        public void Sort() => Raw.Sort((x, y) => x.CompareTo(y));
        public void Sort(IComparer<IniSection>? condExpr) => Raw.Sort(condExpr);

        public void Load(FileInfo iniFile)
        {
            if (iniFile.Exists)
            {
                using var fs = iniFile.OpenRead();
                ParseStream(new StreamReader(fs));
            } else
            {
                throw new FileNotFoundException(iniFile.FullName);
            }
        }
        public void Load(FileInfo iniFile, string encoding = "utf-8")
        {
            if (iniFile.Exists)
            {
                using var fs = iniFile.OpenRead();
                ParseStream(new StreamReader(fs, Encoding.GetEncoding(encoding)));
            } else
            {
                throw new FileNotFoundException(iniFile.FullName);
            }
        }
        public virtual void Save(FileInfo dest, string codec = "utf-8", bool space = false)
        {
            var eq = space ? " = " : "=";

            using var fs = dest.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
            {
                var sw = new StreamWriter(fs, Encoding.GetEncoding(codec));
                foreach (var i in Header)
                    sw.WriteLine(i);
                foreach (var i in Raw)
                {
                    sw.WriteLine(i.ToString());
                    foreach (var j in i)
                        sw.WriteLine(j.ToString(eq));
                }
                sw.Flush();
            }
        }
        private void ParseStream(StreamReader stream)
        // too Python. needs rewritten.
        {
            int cur, max;
            cur = max = Raw.Count == 0 ? 0 : Raw.Count - 1;

            while (!stream.EndOfStream)
            {
                var i = stream.ReadLine();
                var strip = i?.Trim();
                if (string.IsNullOrEmpty(strip))
                {
                    if (cur == -1)
                        Header.Add(string.Empty);
                    else
                        Raw[cur].Add(new());
                    continue;
                }

                switch (strip[0])
                {
                    case '[':
                        var sect = strip.Split(';', 2);
                        var curSect = sect[0].Split(':', 2).Select(i => i.Trim()[1..^1]).ToArray();
                        string? curDesc = sect.Length == 2 ? sect[1] : null;

                        if (!HasSection(curSect[0], out cur))
                        {
                            Raw.Add(new(
                                curSect[0],
                                curSect.Length > 1 ? (HasSection(curSect[1], out int iParent) ? Raw[iParent] : new(curSect[1])) : null,
                                curDesc
                            ));
                            cur = ++max;
                        }
                        break;
                    case ';':
                        if (cur == -1)
                            Header.Add(i);
                        else
                            Raw[cur].Add(new(i));
                        break;
                    default:
                        if (strip.Contains('='))
                        {
                            var spPair = strip.Split('=', 2).ToArray();
                            var spDesc = spPair[1].Split(';', 2);
                            spPair[0] = spPair[0].Trim();

                            var item = new IniItem(
                                spPair[0] == "+" ? $"+{diff++}" : spPair[0],
                                spDesc[0].Trim()
                            );
                            item.Comment = item.Value.IsNull ? spPair[1] : new StringBuilder(spPair[1])
                                .Replace((string)item.Value, string.Empty, 0, spDesc[0].Length)
                                .ToString();
                            Raw[cur].Add(item);
                        }
                        break;
                }
            }

            stream.Close();
        }

        public IEnumerator<IniSection> GetEnumerator() => Raw.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Raw.GetEnumerator();
    }
}
