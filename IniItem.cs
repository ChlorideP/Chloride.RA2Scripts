namespace Chloride.CCINIExt
{
    public class IniItem
    {
        public string? Key;
        public IniValue Value;
        public string? Comment;

        public bool IsEmptyLine => !IsPair && string.IsNullOrEmpty(Comment);
        public bool IsPair => !string.IsNullOrEmpty(Key);
        public bool IsNullValue => Value.IsNull;

        public IniItem() // empty line
        {
            Key = Comment = null;
            Value = new();
        }
        public IniItem(string? key, IniValue value, string? desc = null)
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

        public override string ToString()
        {
            if (!IsPair)
            {
                if (IsEmptyLine) return string.Empty;
                else return $";{Comment}";
            }
            else
            {
                var ret = $"{Key}={Value}";
                if (!string.IsNullOrEmpty(Comment))
                    ret += $" ;{Comment}";
                return ret;
            }
        }

        public string ToString(string delimiterPairing)
        {
            if (!IsPair)
            {
                if (IsEmptyLine) return string.Empty;
                else return $";{Comment}";
            }
            else
            {
                var ret = $"{Key}{delimiterPairing}{Value}";
                if (!string.IsNullOrEmpty(Comment))
                    ret += $" ;{Comment}";
                return ret;
            }
        }
    }
}
