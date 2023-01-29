namespace Chloride.RA2.IniExt;

public struct IniValue
{
    private readonly string raw;

    public IniValue(string? s = null) => raw = s ?? string.Empty;

    public static IniValue Join<T>(IEnumerable<T> seq) where T : notnull => new(string.Join(',', seq));
    public static IniValue Join<T>(params T[] values) where T : notnull => Join(values);
    public string[] Split() => raw.Split(',');

    public static bool operator ==(IniValue v1, IniValue v2) => v1.raw == v2.raw;
    public static bool operator !=(IniValue v1, IniValue v2) => v1.raw != v2.raw;
    public static IniValue operator +(IniValue v1, IniValue v2) => v1.raw + v2.raw;

    public bool IsNull => string.IsNullOrEmpty(raw);

    public T? ConvertTo<T>() => raw is null ? default : (T)Convert.ChangeType(raw, typeof(T));
    public bool ConvertTo() => !string.IsNullOrEmpty(raw)
        && ((new char[] { 'y', 't', '1' }).Contains(char.ToLower(raw[0]))
        || ((new char[] { 'n', 'f', '0' }).Contains(char.ToLower(raw[0]))
        ? false : throw new FormatException($"{raw} is not bool")));
    public static implicit operator IniValue(bool value) => new(value.ToString().ToLower());
    public static implicit operator IniValue(int value) => new(value.ToString());
    public static implicit operator IniValue(double value) => new(value.ToString());
    public static implicit operator IniValue(string? value) => new(value);

    public override string ToString() => raw;
    public override bool Equals(object? obj) => raw.Equals((obj as IniValue?)?.raw);
    public override int GetHashCode() => raw.GetHashCode();
}