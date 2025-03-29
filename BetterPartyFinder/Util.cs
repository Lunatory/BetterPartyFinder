using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace BetterPartyFinder;

public static class Util
{
    public static string UpperCaseStr(this ReadOnlySeString s, sbyte article = 0)
    {
        if (article == 1)
            return s.ExtractText();

        var sb = new StringBuilder(s.ExtractText());
        var lastSpace = true;
        for (var i = 0; i < sb.Length; ++i)
        {
            if (sb[i] == ' ')
            {
                lastSpace = true;
            }
            else if (lastSpace)
            {
                lastSpace = false;
                sb[i]     = char.ToUpperInvariant(sb[i]);
            }
        }

        return sb.ToString();
    }

    internal static bool ContainsIgnoreCase(this string haystack, string needle)
    {
        return CultureInfo.InvariantCulture.CompareInfo.IndexOf(haystack, needle, CompareOptions.IgnoreCase) >= 0;
    }

    internal static IEnumerable<World> WorldsOnDataCentre(IPlayerCharacter character)
    {
        var dcRow = character.HomeWorld.Value.DataCenter.Value.Region;
        return Sheets.WorldSheet.Where(world => world.IsPublic && world.DataCenter.Value.Region == dcRow);
    }

    /// <summary> Iterate over enumerables with additional index. </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static IEnumerable<(T Value, int Index)> WithIndex<T>(this IEnumerable<T> list)
        => list.Select((x, i) => (x, i));

    // From: https://stackoverflow.com/a/1415187
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name != null)
        {
            var field = type.GetField(name);
            if (field != null)
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    return attr.Description;
        }

        return string.Empty;
    }
}