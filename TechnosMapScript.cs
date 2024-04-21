using Chloride.RA2Scripts.Formats;
using Chloride.RA2Scripts.Utils;
using System;

namespace Chloride.RA2Scripts
{
    /*
    # [Infantry]
    # INDEX = OWNER, ID, HEALTH, X, Y, SUB_CELL, MISSION, FACING, TAG, VETERANCY, GROUP, HIGH, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
    # [Units]
    # INDEX = OWNER, ID, HEALTH, X, Y, FACING, MISSION, TAG, VETERANCY, GROUP, HIGH, FOLLOWS_INDEX, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
    # [Aircraft]
    # INDEX = OWNER, ID, HEALTH, X, Y, FACING, MISSION, TAG, VETERANCY, GROUP, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
    # [Structures]
    # INDEX = OWNER,ID,HEALTH,X,Y,FACING,TAG,AI_SELLABLE,AI_REBUILDABLE,POWERED_ON,UPGRADES,SPOTLIGHT,UPGRADE_1,UPGRADE_2,UPGRADE_3,AI_REPAIRABLE,NOMINAL
     */
    internal static class TechnosMapScript
    {
        internal const int FootTypeMissionIndex = 6;
        internal const int TechnoTypeIndex = 1;
        internal const int HealthIndex = 2;

        internal static Random Randomizer = new(114514);

        internal static void FootTypeStatusReplace(IniDoc doc, string section, string status, string? owner = null)
            => IniUtils.ReplaceValue(doc, section, val =>
            {
                if (owner == null || val[0] == owner)
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
    }
}
