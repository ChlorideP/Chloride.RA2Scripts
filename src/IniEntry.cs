using System.Text;

namespace Chloride.RA2.IniExt;
public class IniEntry
{
    public string Key { get; set; }
    public string Value { get; set; }
    public string? Comment { get; set; }

    public IniEntry(string? key = null, string? val = null, string? desc = null)
    {
        Key = key ?? string.Empty;
        Value = val ?? string.Empty;
        Comment = desc;
    }

    public bool IsPair => !string.IsNullOrEmpty(Key);
    public bool IsEmptyLine => !IsPair && string.IsNullOrWhiteSpace(Comment);

    public override string ToString() => ToString(": ");

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
}
