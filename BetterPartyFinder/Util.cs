using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.Sheets;

namespace BetterPartyFinder;

public static class Util
{
    internal static uint MaxItemLevel { get; private set; }

    internal static void CalculateMaxItemLevel()
    {
        if (MaxItemLevel > 0)
            return;

        var max = Plugin.DataManager.GetExcelSheet<Item>()
            .Where(item => item.EquipSlotCategory.Value.Body != 0)
            .Select(item => item.LevelItem.Value.RowId)
            .Max();

        MaxItemLevel = max;
    }

    internal static bool ContainsIgnoreCase(this string haystack, string needle)
    {
        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(haystack, needle, CompareOptions.IgnoreCase) >= 0;
    }

    internal static IEnumerable<World> WorldsOnDataCentre(IPlayerCharacter character)
    {
        var dcRow = character.HomeWorld.Value.DataCenter.Value.Region;
        return Plugin.DataManager.GetExcelSheet<World>().Where(world => world.IsPublic && world.DataCenter.Value.Region == dcRow);
    }

    /// <summary> Iterate over enumerables with additional index. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IEnumerable<(T Value, int Index)> WithIndex<T>(this IEnumerable<T> list)
        => list.Select((x, i) => (x, i));
}