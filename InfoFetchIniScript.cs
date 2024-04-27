using Chloride.RA2Scripts.Formats;
using static Chloride.RA2Scripts.Constants;

namespace Chloride.RA2Scripts
{
    internal static class InfoFetchIniScript
    {
        /// <summary>
        /// To fetch buildings with <c>Factory=</c>.
        /// </summary>
        internal static IEnumerable<string> GetFactoryBuildings(IniDoc mergedRules, bool recurse = false)
            => mergedRules.GetTypeList(BuildingTypes).Where(i => mergedRules.ContainsKey(i, "Factory", recurse));

        /// <summary>
        /// To fetch buildings with SuperWeapon(s). Ares <c>SuperWeapons=</c> supported.
        /// </summary>
        internal static IEnumerable<string> GetSuperWeaponBuildings(IniDoc mergedRules, bool recurse = false)
            => mergedRules.GetTypeList(BuildingTypes).Where(
                i => mergedRules.ContainsKey(i, $"{SuperWeapon}s", recurse)
                || mergedRules.ContainsKey(i, SuperWeapon, recurse)
                || mergedRules.ContainsKey(i, $"{SuperWeapon}2", recurse));
    }
}
