namespace Chloride.RA2Scripts.Formats;

public class IniValue
{
    public string? Value { get; set; }
    public string? Comment { get; set; }

    public static IniValue Join<T>(IEnumerable<T> seq) where T : notnull => new() { Value = string.Join(',', seq) };
    public static IniValue Join<T>(params T[] values) where T : notnull => Join(seq: values);
    public string[] Split() => string.IsNullOrEmpty(Value) ? Array.Empty<string>() : Value.Split(',');

    //public static bool operator ==(IniValue v1, IniValue v2) => v1.Value == v2.Value;
    //public static bool operator !=(IniValue v1, IniValue v2) => v1.Value != v2.Value;
    public static IniValue operator +(IniValue v1, IniValue v2) => v1.Value + ',' + v2.Value;

    public T Convert<T>() where T : struct =>
        string.IsNullOrEmpty(Value) ? default : (T)System.Convert.ChangeType(Value, typeof(T));
    public bool Convert() => !string.IsNullOrEmpty(Value)
        && ((new char[] { 'y', 't', '1' }).Contains(char.ToLower(Value[0]))
        || ((new char[] { 'n', 'f', '0' }).Contains(char.ToLower(Value[0]))
        ? false : throw new FormatException($"{Value} is not bool")));
    public static implicit operator IniValue(bool value) => new() { Value = value.ToString().ToLower() };
    public static implicit operator IniValue(int value) => new() { Value = value.ToString() };
    public static implicit operator IniValue(double value) => new() { Value = value.ToString() };
    public static implicit operator IniValue(string? value) => new() { Value = value };

    public override string ToString() => Value + Comment;
}