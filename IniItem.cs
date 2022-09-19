namespace Chloride.CCINIExt
{
    public struct IniItem
    {
        public string? Key;
        public IniValue Value;
        public string? Comment;

        public bool IsEmptyLine => !IsPair && string.IsNullOrEmpty(Comment);
        public bool IsPair => !string.IsNullOrEmpty(Key);
        public bool IsNullValue => Value.IsNull;

        public IniItem() // empty line
        {
            Key = Comment = string.Empty;
            Value = new();
        }
        public IniItem(string? key, string? val, string? desc = null) // anything not null
        {
            Key = key;
            Value = val;
            Comment = desc;
        }
        public IniItem(string key, IniValue value, string? desc = null) // must be key-val pair
        {
            Key = key;
            Value = value;
            Comment = desc;
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
