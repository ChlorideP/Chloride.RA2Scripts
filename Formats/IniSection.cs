using System.Collections;
using System.Text;

namespace Chloride.RA2Scripts.Formats;

public struct IniParentSection
{
    public string Name = string.Empty;
    public IniSection? Instance = null;
    public IniParentSection() { }
}

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
    // Hint: as C# always use heap to create object,
    // should make "this" updated instead of allocating new one otherwise this track will be lost.
    public IniParentSection Parent { get; set; }

    public IniSection(string name, IniSection? parent = null, string? desc = null)
    {
        Name = name;
        Parent = new()
        {
            Instance = parent,
            Name = parent?.Name ?? string.Empty
        };
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

    internal void Add(string key, IniValue value)
    {
        if (Contains(key, out _))
            items[key] = value;
        else
            items.Add(key, value);
    }
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
                return true;
            sect = sect.Parent.Instance;
        }
        while (recurse && sect is not null);
        value ??= new();
        return false;
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
        if (!string.IsNullOrEmpty(Parent.Name))
            sb.Append($":[{Parent.Name}]");
        if (!string.IsNullOrEmpty(Summary))
            sb.Append($";{Summary}");
        return sb.ToString();
    }
}