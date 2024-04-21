using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;

namespace Chloride.RA2Scripts
{
    /*
    # [Infantry]
    # INDEX = OWNER, ID, HEALTH, X, Y, SUB_CELL, MISSION, FACING, TAG, VETERANCY, GROUP, HIGH, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
    # [Units]
    # INDEX = OWNER, ID, HEALTH, X, Y, FACING, MISSION, TAG, VETERANCY, GROUP, HIGH, FOLLOWS_INDEX, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
    # [Aircraft]
    # INDEX = OWNER, ID, HEALTH, X, Y, FACING, MISSION, TAG, VETERANCY, GROUP, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
     */
    internal static class TechnosMapScript
    {
        internal static void FootTypeStatusReplace(IniDoc doc, string section, int index, string status, string? owner = null)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                if (owner == null || val[0] == owner)
                    val[index] = status;
            });

        internal static void TechnoTypeReplace(IniDoc doc, string section, string src, string dst)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                if (val[1] == src)
                    val[1] = dst;
            });

        internal static void TechnoTypeRandomReplace(IniDoc doc, string section, string src, IEnumerable<string> dst)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                Random rand = new();
                dst = dst.ToArray();
                if (val[1] == src)
                    val[1] = dst.ElementAt(rand.Next(0, dst.Count()));
            });
    }
}
