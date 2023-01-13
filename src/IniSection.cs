using System.Collections;
using System.Text;

namespace Chloride.RA2.IniExt;
public class IniSection : IEnumerable<IniEntry>, IComparable<IniSection>
{
    // Although we all consider ini as a tree, there is 3 elements in an IniEntry.
    // To organize all possible situations (empty lines, comment lines, pair lines), linear storage is necessary.

    // U ask me why support pure comment lines even empty lines?
    // Git differ considers them. And for my convenience I shall SL them properly.
    // It's no way that my ini are in a big mess when script is done.
    private List<IniEntry> items;

    public string Name { get; set; }
    public string? Summary { get; set; }
    /// <summary>
    /// The section myself inherited from.
    /// </summary>
    /// <value>
    /// <para><b>null</b> - Actually have NO base section.</para>
    /// <para><b>Empty Section</b> - Literally have but CANNOT FIND it. Unable to access data without override.</para>
    /// <para><b>Non-empty Section</b> - Have one and got found.</para>
    /// </value>
    public IniSection? Parent { get; set; }

    public IniSection(string name, string? desc = null, IniSection? parent = null)
    {
        Name = name;
        Summary = desc;
        Parent = parent;
        items = new();
    }

    public IniSection(
        string name,
        IEnumerable<IniEntry> source,
        string? desc = null,
        IniSection? parent = null
        ) : this(name, desc, parent)
    {
        items = source.ToList();
    }

    public IniValue this[string key]
    {
        get => !(Contains(key, out IniEntry i) || (Parent?.Contains(key, out i) ?? false))
            ? throw new KeyNotFoundException(key) : i.Value;
        set => Add(key, value);
    }

    public int Count => items.Count;

    public IEnumerable<string> Keys => items.Where(i => !string.IsNullOrEmpty(i.Key)).Select(i => i.Key);
    public IEnumerable<string> Values => items.Where(i => !string.IsNullOrEmpty(i.Key)).Select(i => i.Value);
    public Dictionary<string, IniValue> Items => items.Where(i => !string.IsNullOrEmpty(i.Key)).ToDictionary(i => i.Key, i => new IniValue(i.Value));

    public void Add(string? desc = null) => items.Add(new(desc: desc));
    public void Add<T>(string key, T value, string? desc = null) where T : notnull
    {
        if (!Contains(key, out IniEntry item))
        {
            item.Key = key;
            items.Add(item);
        }
        item.Value = value.ToString() ?? string.Empty;
        item.Comment = desc ?? item.Comment;
    }
    public void AddRange(IEnumerable<IniEntry> items) => this.items.AddRange(items);
    public void AddRange(IDictionary<string, IniValue> dict) => items.AddRange(dict.Select(i => new IniEntry(i.Key, i.Value.ToString())));
    public void Insert(int zbLine, IniEntry item) => items.Insert(zbLine, item);
    // just like Pop(key, fallback) we designed before.
    public bool Remove(string key, out IniValue? val)
    {
        // we resume the repeating key filter, so no need to checkout each one.
        if (Contains(key, out IniEntry e))
        {
            val = e.Value;
            return items.Remove(e);
        }
        else
        {
            val = null;
            return false;
        }
    }
    public bool Remove(IniEntry item) => items.Remove(item);
    public void RemoveAt(int zbLine) => items.RemoveAt(zbLine);
    public void Clear() => items.Clear();
    public bool Contains(string key, out IniEntry entry) => (entry = items.LastOrDefault(i => i.Key == key) ?? new()).IsPair;
    /// <summary>
    /// Deep-copy the section given, and self-update.
    /// </summary>
    internal void Update(IniSection section)
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
        StringBuilder sb = new();
        sb.Append($"[{Name}]");
        if (!string.IsNullOrEmpty(Parent?.Name))
            sb.Append($":[{Parent.Name}]");
        if (!string.IsNullOrEmpty(Summary))
            sb.Append($";{Summary}");
        return sb.ToString();
    }
}