using System.Text.RegularExpressions;

namespace Chloride.CCINIExt
{
    public struct IniValue
    {
        internal string raw;

        public IniValue(string? s) => raw = (s is null ? string.Empty : s);
        public IniValue() => raw = string.Empty;

        public string[] TrySplit()
        {
            var re = new Regex(",+");
            return re.IsMatch(raw) ? re.Split(raw) : throw new FormatException($"{raw} is not array");
        }

        public bool IsNull => string.IsNullOrEmpty(raw);

        public static explicit operator bool(IniValue value) =>
            value.raw is not null
            && ((new char[] { 'y', 't', '1' }).Contains(char.ToLower(value.raw[0]))
            || ((new char[] { 'n', 'f', '0' }).Contains(char.ToLower(value.raw[0]))
            ? false : throw new FormatException($"{value.raw} is not bool")));
        public static implicit operator IniValue(bool value) => new(value.ToString().ToLower());

        public static explicit operator int(IniValue value) => int.Parse(value.raw);
        public static implicit operator IniValue(int value) => new(value.ToString());

        public static explicit operator double(IniValue value) => double.Parse(value.raw);
        public static explicit operator float(IniValue value) => float.Parse(value.raw);
        public static implicit operator IniValue(double value) => new(value.ToString());

        public static explicit operator string(IniValue value) => value.raw;
        public static implicit operator IniValue(string? value) => new(value);

        public override string ToString() => raw;
    }
}