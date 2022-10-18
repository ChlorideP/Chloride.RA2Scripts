using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Chloride.CCiniExt
{
    public struct IniValue
    {
        private readonly string raw;

        public IniValue(string? s) => raw = s ?? string.Empty;
        public IniValue() => raw = string.Empty;

        public string[] TrySplit()
        {
            var re = new Regex(",+");
            return re.IsMatch(raw) ? re.Split(raw) : throw new FormatException($"{raw} is not array");
        }

        public static bool operator ==(IniValue v1, IniValue v2) => v1.raw == v2.raw;
        public static bool operator !=(IniValue v1, IniValue v2) => v1.raw != v2.raw;

        public bool IsNull => string.IsNullOrEmpty(raw);

        public static explicit operator bool(IniValue value) =>
            !value.IsNull
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
        public override bool Equals(object? obj) => raw.Equals((obj as IniValue?)?.raw);
        public override int GetHashCode() => raw.GetHashCode();
    }
}