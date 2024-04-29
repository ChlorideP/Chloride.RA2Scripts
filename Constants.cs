namespace Chloride.RA2Scripts;
public static class Constants
{
    /*
     * [Infantry]
     * INDEX = OWNER, ID, HEALTH, X, Y, SUB_CELL, MISSION, FACING, TAG, VETERANCY, GROUP, HIGH, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
     * [Units]
     * INDEX = OWNER, ID, HEALTH, X, Y, FACING, MISSION, TAG, VETERANCY, GROUP, HIGH, FOLLOWS_INDEX, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
     * [Aircraft]
     * INDEX = OWNER, ID, HEALTH, X, Y, FACING, MISSION, TAG, VETERANCY, GROUP, AUTOCREATE_NO_RECRUITABLE, AUTOCREATE_YES_RECRUITABLE
     * [Structures]
     * INDEX = OWNER, ID, HEALTH, X, Y, FACING, TAG,AI_SELLABLE, AI_REBUILDABLE, POWERED_ON, UPGRADES, SPOTLIGHT, UPGRADE_1, UPGRADE_2, UPGRADE_3, AI_REPAIRABLE, NOMINAL
     */
    internal const int FootTypeMissionIndex = 6;
    internal const int TechnoTypeIndex = 1;
    internal const int HealthIndex = 2;
    internal const int HouseIndex = 0;

    public const string BuildingTypes = nameof(BuildingTypes);
    public const string InfantryTypes = nameof(InfantryTypes);
    public const string VehicleTypes = nameof(VehicleTypes);
    public const string AircraftTypes = nameof(AircraftTypes);
    public const string SuperWeapon = nameof(SuperWeapon);
}