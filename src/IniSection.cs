using System.Collections;

namespace Chloride.RA2.IniExt
{
    // List (x)
    // Dictionary (o)
    public class IniSection : IEnumerable<IniEntry>, IComparable<IniSection>
    {
        private List<IniEntry> items = new();

        public IniValue this[string key]
        {
            get => !(Contains(key, out IniEntry i) || (Parent?.Contains(key, out i) ?? false))
                ? throw new KeyNotFoundException(key) : i.Value;
            set => Add(key, value);
        }

        public string Name { get; set; }
        public string? Summary { get; set; }

        /// <summary>
        /// The section myself inherited from.
        /// </summary>
        /// <value>
        /// <para>3 possible situations:</para>
        /// <para><b>null</b> - Actually have NO base section.</para>
        /// <para><b>Empty Section</b> - Literally have but CANNOT FIND it. Unable to access data without override.</para>
        /// <para><b>Non-empty Section</b> - Have one and got found.</para>
        /// </value>
        public IniSection? Parent { get; set; }

        public IniSection(string name, IniSection? super = null, string? desc = null)
        {
            Name = name;
            Parent = super;
            Summary = desc;
        }
        // filter
        public IniSection(string name, IEnumerable<IniEntry> source) : this(name) =>
            items.AddRange(source.Where(i => i.IsPair).Select(i =>
            {
                i.Comment = null;
                return i;
            }));

        public int Count => items.Count;

        public IEnumerable<string> Keys => items.Where(i => !string.IsNullOrEmpty(i.Key)).Select(i => i.Key);
        public IEnumerable<IniValue> Values => items.Where(i => !string.IsNullOrEmpty(i.Key)).Select(i => i.Value);
        public Dictionary<string, IniValue> Items => items.Where(i => !string.IsNullOrEmpty(i.Key)).ToDictionary(i => i.Key, i => i.Value);

        public void Add(string? desc = null) => items.Add(new(desc));
        public void Add(string key, IniValue value, string? desc = null)
        {
            if (!Contains(key, out IniEntry item))
            {
                item.Key = key;
                items.Add(item);
            }
            item.Value = value;
            item.Comment = desc ?? item.Comment;
        }
        public void AddRange(IEnumerable<IniEntry> items) => this.items.AddRange(items);
        public void AddRange(IDictionary<string, IniValue> dict) => items.AddRange(dict.Select(i => new IniEntry(i.Key, i.Value)));
        public void Insert(int zbLine, IniEntry item) => items.Insert(zbLine, item);
        public bool Remove(string key, bool recurse = false)
        {
            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (items[i].Key == key)
                    items.RemoveAt(i);
            }
            if (recurse && (Parent?.Contains(key, out _) ?? false))
                Add(key, string.Empty);
            return !Contains(key, out _);
        }
        public bool Remove(IniEntry item) => items.Remove(item);
        public void RemoveAt(int zbLine) => items.RemoveAt(zbLine);
        public void Clear() => items.Clear();
        public bool Contains(string key, out IniEntry item) => (item = items.LastOrDefault(i => i.Key == key) ?? new()).IsPair;
        public string? GetValue(string key) => Contains(key, out IniEntry iKey) || (Parent?.Contains(key, out iKey) ?? false) ? iKey.Value.ToString() : null;

        /// <summary>
        /// Deep-copy the section given, and self-update.
        /// </summary>
        public void Update(IniSection section)
        {
            Name = section.Name;
            Summary = section.Summary;
            Parent = section.Parent;
            items = section.ToList();
        }
        public int CompareTo(IniSection? other) => Name.CompareTo(other?.Name);
        public IEnumerator<IniEntry> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public override string ToString()
        {
            var ret = $"[{Name}]";
            if (Parent != null)
                ret += $":[{Parent.Name}]";
            if (!string.IsNullOrEmpty(Summary))
                ret += $";{Summary}";
            return ret;
        }
    }
}
