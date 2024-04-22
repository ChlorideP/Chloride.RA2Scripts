using Chloride.RA2Scripts.Formats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloride.RA2Scripts
{
    internal static class InfoFetchIniScript
    {
        internal const string BuildingTypes = nameof(BuildingTypes);
        internal const string SuperWeapon = nameof(SuperWeapon);

        internal static IEnumerable<string> GetFactoryBuildings(IniDoc mergedRules, bool recurse = false)
            => mergedRules.GetTypeList(BuildingTypes).Where(i => mergedRules.ContainsKey(i, "Factory", recurse));

        internal static IEnumerable<string> GetSuperWeaponBuildings(IniDoc mergedRules, bool recurse = false)
            => mergedRules.GetTypeList(BuildingTypes).Where(
                i => mergedRules.ContainsKey(i, $"{SuperWeapon}s", recurse)
                || mergedRules.ContainsKey(i, SuperWeapon, recurse)
                || mergedRules.ContainsKey(i, $"{SuperWeapon}2", recurse));
    }
}
