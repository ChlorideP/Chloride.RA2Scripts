namespace Chloride.CCiniExt
{
    public class IniItem
    {
        public string Key { get; set; }
        public IniValue Value { get; set; }
        public string? Comment { get; set; }

        public IniItem(string? desc = null) // empty line || description only
        {
            Key = string.Empty;
            Value = new();
            Comment = desc;
        }
        public IniItem(string key, IniValue value, string? desc = null)
        {
            Key = key;
            Value = value;
            Comment = desc;
        }

        public bool IsPair => !string.IsNullOrEmpty(Key);
        public bool IsEmptyLine => !IsPair && string.IsNullOrEmpty(Comment);

        public override string ToString() => ToString(": ");
        public string ToString(string pairLinker = "=") => IsPair ? $"{Key}{pairLinker}{Value}{Comment}" : $"{Comment}";
    }
}
