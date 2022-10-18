using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Chloride.CCiniExt
{
    public static class IniSerializer
    {
        static IniSerializer() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        public static void Deserialize(this IniDoc doc, FileInfo ini)
        {
            if (!ini.Exists)
                throw new FileNotFoundException(ini.FullName);
            using var fs = ini.OpenRead();
            using var sr = new StreamReader(fs);
            Deserialize(doc, sr);
        }

        public static void Deserialize(IniDoc doc, TextReader tr)
        {
            int cur, max;
            cur = max = doc.Count == 0 ? 0 : doc.Count - 1;

            bool HasSection(string name, out int idx) => (idx = doc.Raw.Select(i => i.Name).ToList().IndexOf(name)) != -1;

            while (tr.Peek() > 0)
            {
                var line = tr.ReadLine();
                var strip = line?.Trim();

                if (string.IsNullOrEmpty(strip))
                {
                    if (cur == -1)
                        doc.Head.Add(string.Empty);
                    else
                        doc.Raw[cur].Add();
                    continue;
                }

                switch (strip.First())
                {
                    case '[':
                        var sect = strip.Split(';', 2);
                        var curSect = sect.First().Split(':').Select(i => i.Trim()[1..^1]).ToArray();
                        var curDesc = sect.ElementAtOrDefault(1);

                        if (!HasSection(curSect[0], out cur))
                        {
                            doc.Raw.Add(new(
                                curSect[0],
                                curSect.Length > 1 ? (HasSection(curSect[1], out int iParent) ? doc.Raw[iParent] : new(curSect[1])) : null,
                                curDesc
                            ));
                            cur = ++max;
                        }
                        break;
                    case ';':
                        if (cur == -1)
                            doc.Head.Add(line!);
                        else
                            doc.Raw[cur].Add(new(line));
                        break;
                    default:
                        if (strip.Contains('='))
                        {
                            var pair = strip.Split('=', 2);

                            var key = pair[0].Trim();
                            IniValue val = pair[1].Split(';', 2)[0].Trim();
                            var desc = val.IsNull ? pair[1] : new StringBuilder(pair[1])
                                .Replace(val.ToString(), string.Empty, 0, pair[1].IndexOf(';') + 1)
                                .ToString();

                            doc.Raw[cur].Add(key, val, desc);
                        }
                        break;
                }
            }

            tr.Close();
        }

        public static void Serialize(this IniDoc doc, FileInfo iniFile, string encoding = "utf-8", string pairing = "=")
        {
            using var fs = iniFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
            using StreamWriter sw = new(fs, Encoding.GetEncoding(encoding)); // DON'T forget gb2312, esp. MAP file from FA2.

            Serialize(doc, sw, pairing);
        }

        public static void Serialize(IniDoc doc, TextWriter tw, string pairing = "=")
        {
            foreach (var i in doc.Head)
                tw.WriteLine(i);

            foreach (var i in doc)
            {
                tw.WriteLine(i.ToString());
                foreach (var j in i)
                    tw.WriteLine(j.ToString(pairing));
            }
        }
    }
}
