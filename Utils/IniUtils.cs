using Chloride.RA2Scripts.Formats;

namespace Chloride.RA2Scripts.Utils
{
    public static class IniUtils
    {
        /// <summary>
        /// Hint: Dictionary keys are readonly and not able to override.
        /// </summary>
        public static void IteratePairs(IniDoc doc, string section, Action<string, IniValue> action, bool overrideValue = false)
        {
            if (!doc.Contains(section, out IniSection? sect))
                return;
            foreach (var i in sect!)
            {
                if (i.Key.StartsWith(';'))  // comments
                    continue;
                IniValue val = i.Value;
                action.Invoke(i.Key, val);
                if (overrideValue)
                    sect[i.Key] = val;
            }
        }
        public static void ReplaceValue(IniDoc doc, string section, Action<string[]> action)
            => IteratePairs(doc, section, (key, val) =>
            {
                var seq = val.Split();
                action.Invoke(seq);
                val = IniValue.Join(seq);
            }, true);

        public static IniDoc ReadIni(FileInfo file, bool include = false)
        {
            var paths = include ? IniSerializer.TryGetIncludes(file.FullName) : new() { file };
            var ret = new IniDoc();
            ret.Deserialize(paths.Where(i => i.Exists).ToArray());
            return ret;
        }
    }
}
