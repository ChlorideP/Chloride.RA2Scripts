using System.Collections;
using System.Collections.ObjectModel;
using System.Text;

namespace Chloride.CCiniExt
{
    public class IniDoc : IEnumerable<IniSection>
    {
        // place here to support "+=" in multiple inis.
        internal int diff = 0;

        public IniDoc()
        {
            Head = new();
            Raw = new();
        }

        internal List<string> Head { get; private set; }
        internal List<IniSection> Raw { get; private set; }

        public IniSection this[string sectionName]
        {
            get => Contains(sectionName, out IniSection? section)
                ? section! : throw new KeyNotFoundException(sectionName);
            set
            {
                // keep assignment corresponding.
                value.Name = sectionName;

                if (Contains(sectionName, out IniSection? section))
                    section!.Update(value);
                else
                    Raw.Add(value);
            }
        }
        public string? this[string section, string key]
        {
            get => this[section].GetValue(key);
            set => this[section][key] = value;
        }

        public int Count => Raw.Count;

        /// <summary>
        /// Add a new section without trying to replace the existing one.
        /// </summary>
        public void Add(string sectionName)
        {
            if (!Contains(sectionName, out _))
                Raw.Add(new(sectionName));
        }

        public bool Remove(string sectionName)
            => Contains(sectionName, out IniSection? target) && Raw.Remove(target!);

        public void Rename(string _old, string _new)
        {
            if (Contains(_old, out IniSection? old))
            {
                if (_old == _new || Contains(_new, out _))
                    throw new ArgumentException($"Section {_new} already exists");
                old!.Name = _new;
            }
        }

        /// <summary>
        /// Remove all empty lines and comments.
        /// </summary>
        public void CleanUp()
        {
            Head.Clear();
            foreach (var section in Raw)
            {
                var items = section.Items;
                section.Clear();
                section.AddRange(items);
            }
        }

        /// <summary>
        /// Remove all stored data.
        /// </summary>
        public void Clear()
        {
            Head.Clear();
            Raw.Clear();
        }

        public bool Contains(string sectionName, out IniSection? section)
            => (section = Raw.FirstOrDefault(i => i.Name == sectionName)) != null;

        public string[] GetTypeList(string sect) => Contains(sect, out IniSection? ret)
            ? ret!.Values.Select(i => i.ToString()).ToArray()
            : Array.Empty<string>();

        public IEnumerator<IniSection> GetEnumerator() => Raw.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Raw.GetEnumerator();
    }
}
