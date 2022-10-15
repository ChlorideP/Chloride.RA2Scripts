namespace Chloride.CCiniExt
{
    public class IniItem
    {
        public string Key;
        public IniValue Value;
        public string? Comment;

        public bool IsEmptyLine => !IsPair && string.IsNullOrEmpty(Comment);
        public bool IsPair => !string.IsNullOrEmpty(Key);

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
        public IniItem(KeyValuePair<string, IniValue> pair)
        {
            Key = pair.Key;
            Value = pair.Value;
            Comment = null;
        }

        public string ToString(string delimiterPairing = "=") => IsPair ? $"{Key}{delimiterPairing}{Value}{Comment}" : $"{Comment}";
    }
}
