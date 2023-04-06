using System.Collections;
using System.Text;

namespace Chloride.RA2.IniExt;
public class IniSection : IEnumerable<KeyValuePair<string, IniValue>>, IComparable<IniSection>
{
    // Actually it does more finding operation instead of rearrangement.
    // just make Tn = O(1) by using hashtable (or dict).
    private Dictionary<string, IniValue> items = new();

    public string Name { get; set; }
    public string? Summary { get; set; }
    /// <summary>
    /// The section myself inherited from.
    /// </summary>
    /// <value>
    /// <para><b>null</b> - Actually have NO base section.</para>
    /// <para><b>Empty Section</b> - Literally have but CANNOT FIND it.</para>
    /// <para><b>Non-empty Section</b> - Have one and got found.</para>
    /// </value>

    // Hint: as C# always use heap to create object,
    // should make "this" updated instead of allocating new one otherwise this track will be lost.
    public IniSection? Parent { get; set; }

    public IniSection(string name, IniSection? parent = null, string? desc = null)
    {
        Name = name;
        Parent = parent;
        Summary = desc;
    }
    public IniSection(
        string name,
        IDictionary<string, string?> pairs,
        IniSection? parent = null,
        string? desc = null)
        : this(name, parent, desc)
    {
        items = pairs.ToDictionary(i => i.Key, i => new IniValue() { Value = i.Value });
    }

    public IniValue this[string key]
    {
        get => Contains(key, out IniValue value, true) ? value : throw new KeyNotFoundException(key);
        set => Add(key, value);
    }

    public int Count => items.Count;

    public IEnumerable<string> Keys => items.Keys.Where(i => !(i.StartsWith(';') || i.StartsWith('#')));
    public IEnumerable<string> Values => Keys.Select(i => items[i].Value!);
    public IDictionary<string, string?> Items => Keys.ToDictionary(i => i, i => items[i].Value);

    internal void Add(string key, IniValue value) => items.Add(key, value);
    public void Add<T>(string key, T value, string? desc = null) where T : notnull
    {
        if (!Contains(key, out IniValue val))
            items.Add(key, val);
        val.Value = value.ToString();
        val.Comment = desc ?? val.Comment;
    }
    /// <summary>
    /// Remove a key-value entry in this section.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the entry successfully removed,
    /// <c>false</c> otherwise, for example, key was found in its Parent(s).
    /// </returns>
    public bool Remove(string key, bool recursive = false)
        => Contains(key, out _, recursive) && items.Remove(key);

    public bool Contains(string key, out IniValue value, bool recurse = false)
    {
        var sect = this;
        do
        {
            if (sect.items.TryGetValue(key, out value!))
                break;
            sect = sect.Parent;
        }
        while (recurse && sect?.Count > 0);
        value ??= new();
        return sect?.Count > 0;
    }

    public void Clear() => items.Clear();

    /// <summary>
    /// Deep-copy the section given, and self-update.
    /// </summary>
    internal void Update(IniSection section)
    {
        Name = section.Name;
        Summary = section.Summary;
        Parent = section.Parent;
        // not sure whether the old instance would be free
        // if just "items = section.items" ...
        items = section.ToDictionary(i => i.Key, i => i.Value);
    }

    public int CompareTo(IniSection? other) => Name.CompareTo(other?.Name);
    public IEnumerator<KeyValuePair<string, IniValue>> GetEnumerator() => items.GetEnumerator();
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