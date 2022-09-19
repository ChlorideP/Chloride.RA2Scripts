using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;

namespace Chloride.CCINIExt
{
    // List (x)
    // Dictionary (o)
    public class IniSection : IList<IniItem>, IComparable<IniSection>
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
        public IniSection(string section, IDictionary<string, string> source) // with source given
        {
            Name = section;
            Concat(source);
        }

        public int Count => items.Count;
        public bool IsReadOnly => false;

        public IniItem this[int line] { get => items[line]; set => items[line] = value; }
        public IniValue this[string key]
        {
            get
            {
                IniItem i;
                if (!(ContainsKey(key, out i) && (Parent?.ContainsKey(key, out i) ?? false)))
                    throw new KeyNotFoundException(key);
                return i.Value;
            }
            set => Add(key, value);
        }

        public IEnumerator<IniItem> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public void Concat<T>(IDictionary<string, T> source) where T : notnull
        {
            foreach (var item in source)
                items.Add(new(
                    item.Key,
                    item.Value?.ToString()
                ));
        }

        public int IndexOf(IniItem item) => items.IndexOf(item);

        public void Insert(int line, IniItem item) => items.Insert(line, item);
        public void Insert<T>(int line, string key, T value) where T : notnull
            => items.Insert(line, new(key, value?.ToString()));

        public bool Remove(IniItem item) => items.Remove(item);
        public bool RemoveKey(string key)
        {
            IniItem item;
            if (ContainsKey(key, out item))
                return items.Remove(item);
            else if (Parent?.ContainsKey(key, out _) ?? false)
                Add(key, string.Empty);
            return false;
        }
        public void RemoveAt(int line) => items.RemoveAt(line);

        public void Add(IniItem item) => items.Add(item);
        public void Add(string key, IniValue value, string? desc = null)
        {
            IniItem item;
            if (ContainsKey(key, out item))
            {
                item.Value = value;
                item.Comment = desc;
            }
            else
                Add(new(key, value, desc));
        }
        public void Add<T>(string key, T value, string? desc = null) where T : notnull
        {
            IniItem item;
            if (ContainsKey(key, out item))
            {
                item.Value = value.ToString();
                item.Comment = desc;
            }
            else
                Add(new(key, value.ToString(), desc));
        }

        public void Clear() => items.Clear();

        public bool Contains(IniItem item) => items.Contains(item);
        public bool ContainsKey(string key, out IniItem item)
        {
            foreach (var i in items)
            {
                if (i.Key == key)
                {
                    item = i;
                    return true;
                }
            }
            item = default;
            return Parent?.ContainsKey(key, out item) ?? false;
        }

        public string GetValue(string key)
        {
            IniItem iKey;
            if (ContainsKey(key, out iKey) || (Parent?.ContainsKey(key, out iKey) ?? false))
                return iKey.Value.ToString();
            else
                return string.Empty;
        }

        public List<string> Keys()
        {
            List<string> ret = new();
            foreach (var i in items)
            {
                if (!string.IsNullOrEmpty(i.Key))
                    ret.Add(i.Key);
            }
            return ret;
        }

        public List<IniValue> Values()
        {
            List<IniValue> ret = new();
            foreach (var i in items)
            {
                if (!string.IsNullOrEmpty(i.Key))
                    ret.Add(i.Value);
            }
            return ret;
        }

        public Dictionary<string, IniValue> Items()
        {
            Dictionary<string, IniValue> ret = new();
            foreach (var i in items)
            {
                if (!string.IsNullOrEmpty(i.Key))
                    ret.Add(i.Key, i.Value);
            }
            return ret;
        }

        public void CopyTo(IniItem[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public override string ToString()
        {
            var ret = $"[{Name}]";
            if (Parent != null)
                ret += $":[{Parent.Name}]";
            if (Description != null)
                ret += $";{Description}";
            return ret;
        }

        public int CompareTo(IniSection? other)
        {
            if (other == null)
                return -1;
            else
                return Name.CompareTo(other.Name);
        }
    }
}
