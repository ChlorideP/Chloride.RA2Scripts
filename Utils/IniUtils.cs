using Chloride.RA2Scripts.Formats;

namespace Chloride.RA2Scripts.Utils
{
    internal static class IniUtils
    {
        internal static void ReplaceValue(IniDoc doc, string section, Action<string[]> action)
        {
            if (!doc.Contains(section, out IniSection? sect))
                return;
            foreach (var i in sect!)
            {
                if (i.Key.StartsWith(';'))  // comments
                    continue;
                var val = i.Value.Split();
                action.Invoke(val);
                sect[i.Key] = IniValue.Join(val);
            }
        }

        public static IniDoc ReadIni(FileInfo file, bool include = false)
        {
            var paths = include ? IniSerializer.TryGetIncludes(file.FullName) : new() { file };
            var ret = new IniDoc();
            ret.Deserialize(paths.Where(i => i.Exists).ToArray());
            return ret;
        }
    }
}
