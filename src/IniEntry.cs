using System.Text;

namespace Chloride.RA2.IniExt;
public class IniEntry
{
    public string Key { get; set; }
    public string? Value { get; set; }
    public string? Comment { get; set; }

    public IniEntry(string? key = null, string? val = null, string? desc = null)
    {
        Key = key ?? string.Empty;
        Value = val;
        Comment = desc;
    }

    public bool IsPair => !string.IsNullOrEmpty(Key);
    public bool IsEmptyLine => !IsPair && string.IsNullOrWhiteSpace(Comment);

    public string ToString(string pairLinker = "=")
    {
        StringBuilder sb = new();
        sb.Append(Key);
        if (IsPair)
        {
            sb.Append(pairLinker);
            sb.Append(Value);
        }
        sb.Append(Comment);
        return sb.ToString();
    }

    public T? GetValue<T>() => Value is null ? default : (T)Convert.ChangeType(Value, typeof(T));
    public bool GetValue() => !string.IsNullOrEmpty(Value)
        && ((new char[] { 'y', 't', '1' }).Contains(char.ToLower(Value[0]))
        || ((new char[] { 'n', 'f', '0' }).Contains(char.ToLower(Value[0]))
        ? false : throw new FormatException($"{Value} is not bool")));
    public void SetValue<T>(T value) => Value = value?.ToString();
    public void SetValue(bool value) => Value = value.ToString().ToLower();
}
