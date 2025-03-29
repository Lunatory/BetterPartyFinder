using System.Linq;
using Lumina.Excel;
using Lumina.Excel.Sheets;

namespace BetterPartyFinder;

public static class Sheets
{
    private static readonly uint[] AllowedContentTypes =
    [
        2,   // Dungeons
        3,   // Guildhests
        4,   // Trials
        5,   // Raids
        6,   // PvP
        16,  //
        21,  // Deep Dungeons
        26,  // Eureka
        28,  // Ultimate Raids
        30,  // V&C Dungeon Finder
        37   // Chaotic
    ];

    public static readonly ExcelSheet<Item> ItemSheet;
    public static readonly ExcelSheet<Addon> AddonSheet;
    public static readonly ExcelSheet<World> WorldSheet;
    public static readonly ExcelSheet<ContentType> ContentTypeSheet;
    public static readonly ExcelSheet<ContentRoulette> ContentRouletteSheet;
    public static readonly ExcelSheet<ContentFinderCondition> ContentFinderSheet;

    public static readonly uint MaxItemLevel;

    public static readonly ContentFinderCondition[] DutyCache;

    static Sheets()
    {
        ItemSheet = Plugin.Data.GetExcelSheet<Item>();
        AddonSheet = Plugin.Data.GetExcelSheet<Addon>();
        WorldSheet = Plugin.Data.GetExcelSheet<World>();
        ContentTypeSheet = Plugin.Data.GetExcelSheet<ContentType>();
        ContentRouletteSheet = Plugin.Data.GetExcelSheet<ContentRoulette>();
        ContentFinderSheet = Plugin.Data.GetExcelSheet<ContentFinderCondition>();

        // maximum item level (based on max chestpiece)
        MaxItemLevel = ItemSheet
            .Where(item => item.EquipSlotCategory.Value.Body != 0)
            .Select(item => item.LevelItem.Value.RowId)
            .Max();

        // Fill duty cache
        DutyCache = ContentFinderSheet
            .Where(cf => cf.Unknown47) // Unknown47 = IsInUse, is False for instances that aren't exist anymore
            .Where(cf => AllowedContentTypes.Contains(cf.ContentType.RowId))
            .ToArray();
    }
}