using System.Collections;

namespace Chloride.CCiniExt
{
    // List (x)
    // Dictionary (o)
    public class IniSection : IEnumerable<IniItem>, IComparable<IniSection>
    {
        private List<IniItem> items = new();

        public string Name;
        public IniSection? Parent;
        public string? Description;

        public IniSection(string section, IniSection? super = null, string? desc = null) // init empty
        {
            Name = section;
            Parent = super;
            Description = desc;
        }
        public IniSection(string section, IDictionary<string, IniValue> source) // with source given
        {
            Name = section;
            AddRange(source);
        }

        public int Count => items.Count;

        public IniItem this[int line] { get => items[line]; set => items[line] = value; }
        public IniValue this[string key]
        {
            get => !(Contains(key, out IniItem i) && (Parent?.Contains(key, out i) ?? false))
                ? throw new KeyNotFoundException(key) : i.Value;
            set => Add(key, value);
        }

        public IEnumerator<IniItem> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public void Insert(int line, IniItem item) => items.Insert(line, item);
        public void Insert(int line, string key, IniValue value) => items.Insert(line, new(key, value));

        public bool Remove(IniItem item) => items.Remove(item);
        public bool Remove(string key, bool recurse = false)
        {
            foreach (var i in items)
            {
                if (i.Key == key)
                    items.Remove(i);
            }
            if (recurse && (Parent?.Contains(key, out _) ?? false))
                Add(key, string.Empty);
            return !Contains(key, out _);
        }
        public void RemoveAt(int line) => items.RemoveAt(line);

        public void Add(IniItem item) => items.Add(item);
        public void Add(string key, IniValue value, string? desc = null)
        {
            if (!Contains(key, out IniItem item))
            {
                item.Key = key;
                Add(item);
            }
            item.Value = value;
            item.Comment = desc ?? item.Comment;
        }
        public void AddRange(IEnumerable<IniItem> sequence) => items.AddRange(sequence);
        public void AddRange(IDictionary<string, IniValue> source) => items.AddRange(source.Select(i => new IniItem(i)));

        public void Clear() => items.Clear();

        public bool Contains(string key, out IniItem item) =>
            (item = items.LastOrDefault(i => i.Key == key) ?? new()).IsPair || (Parent?.Contains(key, out item) ?? false);

        public string GetValue(string key, string fallback = "") => Contains(key, out IniItem iKey) ? iKey.Value.ToString() : fallback;

        public IEnumerable<string> Keys => items.Select(i => i.Key ?? string.Empty).Where(i => !string.IsNullOrEmpty(i));

        public IEnumerable<IniValue> Values => items.Where(i => !string.IsNullOrEmpty(i.Key)).Select(i => i.Value);

        public Dictionary<string, IniValue> Items => items.Where(i => !string.IsNullOrEmpty(i.Key)).ToDictionary(i => i.Key!, i => i.Value);

        public override string ToString()
        {
            var ret = $"[{Name}]";
            if (Parent != null)
                ret += $":[{Parent.Name}]";
            if (!string.IsNullOrEmpty(Description))
                ret += $";{Description}";
            return ret;
        }

        public int CompareTo(IniSection? other) => Name.CompareTo(other?.Name);
    }
}
