using Chloride.RA2Scripts.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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
        private static void Replacement(IniDoc doc, string section, Action<string[]> action)
        {
            if (!doc.Contains(section, out IniSection? sect))
                return;
            foreach (var i in sect!.Keys)
            {
                var val = sect[i].Split();
                action.Invoke(val);
                sect[i] = IniValue.Join(val);
            }
        }
        internal static void FootTypeStatusReplace(IniDoc doc, string section, int index, string status, string? owner = null)
            => Replacement(doc, section, val =>
            {
                if (owner == null || val[0] == owner)
                    val[index] = status;
            });

        internal static void TechnoTypeReplace(IniDoc doc, string section, string src, string dst)
            => Replacement(doc, section, val =>
            {
                if (val[1] == src)
                    val[1] = dst;
            });

        internal static void TechnoTypeRandomReplace(IniDoc doc, string section, string src, IEnumerable<string> dst)
            => Replacement(doc, section, val =>
            {
                Random rand = new();
                dst = dst.ToArray();
                if (val[1] == src)
                    val[1] = dst.ElementAt(rand.Next(0, dst.Count()));
            });
    }
}
