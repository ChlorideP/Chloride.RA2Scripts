using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;
using static Chloride.RA2Scripts.Constants;

namespace Chloride.RA2Scripts
{
    internal static class TechnosMapScript
    {
        internal static Random Randomizer = new(114514);

        internal static void FootTypeStatusReplace(IniDoc doc, string section, string status, string? owner = null, string? techno = null)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                if (techno != null && val[TechnoTypeIndex] != techno)
                    return;
                if (owner == null || val[HouseIndex] == owner)
                    val[FootTypeMissionIndex] = status;
            });

        internal static void TechnoTypeReplace(IniDoc doc, string section, string src, string dst)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                if (val[TechnoTypeIndex] == src)
                    val[TechnoTypeIndex] = dst;
            });

        internal static void TechnoRandomType(IniDoc doc, string section, string src, IEnumerable<string> dst)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                dst = dst.ToArray();
                if (val[TechnoTypeIndex] == src)
                    val[TechnoTypeIndex] = dst.ElementAt(Randomizer.Next(0, dst.Count()));
            });

        // random facing.fscript
        internal static void TechnoRandomFacing(IniDoc doc, string section)
        {
            int idx = section switch
            {
                "Infantry" => 7,
                _ => 5
            };
            IniUtils.ReplaceValue(doc, section, val => val[idx] = Randomizer.Next(0, 256).ToString());
        }

        // random HP.fscript
        internal static void TechnoRandomHP(IniDoc doc, string section)
            => IniUtils.ReplaceValue(doc, section, val => val[HealthIndex] = Randomizer.Next(0, 256).ToString());

        /// <summary>
        /// Lazy Ini writer for preventing engineers capturing specific buildings.
        /// </summary>
        /// <param name="mergedRules">If not <c>null</c>, it would first search the building type in <c>mergedRules</c>, to consider if necessary to write.</param>
        /// <param name="recurse">If <c>true</c>, it would do recursive search in <c>mergedRules</c>, since Ares supports section inheritance.</param>
        internal static void InhibitCapturable(IniDoc doc, IEnumerable<string> todoList, IniDoc? mergedRules = null, bool recurse = false)
        {
            if (!doc.Contains("Structures", out IniSection? buildings))
                return;
            var presets = buildings!.Values.Select(i => i.Split(',')[TechnoTypeIndex]).ToHashSet();
            foreach (var i in todoList)
            {
                if (!presets.Contains(i))
                    continue;
                if (mergedRules == null || (mergedRules.ContainsKey(i, "Capturable", recurse) && mergedRules[i]["Capturable"].Convert())) // capturable def to false.
                {
                    doc.Add(i);
                    doc[i]["Capturable"] = false;
                }
            }
        }
    }
}
