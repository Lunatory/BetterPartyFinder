using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
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

    private void DrawDutiesTab(ConfigurationFilter filter)
    {
        using var tabItem = ImRaii.TabItem("Duties");
        if (!tabItem.Success)
            return;

        var listModeStrings = new[]
        {
            "Show ONLY these duties",
            "Do NOT show these duties",
        };
        var listModeIdx = filter.DutiesMode == ListMode.Blacklist ? 1 : 0;
        ImGui.TextUnformatted("List mode");
        ImGui.PushItemWidth(-1);
        if (ImGui.Combo("###list-mode", ref listModeIdx, listModeStrings, listModeStrings.Length))
        {
            filter.DutiesMode = listModeIdx == 0 ? ListMode.Whitelist : ListMode.Blacklist;
            Plugin.Config.Save();
        }
        ImGui.PopItemWidth();

        var query = DutySearchQuery;
        ImGui.TextUnformatted("Search");
        if (ImGui.InputText("###search", ref query, 1_000))
        {
            DutySearchQuery = query;
        }

        ImGui.SameLine();
        if (ImGui.Button("Clear list"))
        {
            filter.Duties.Clear();
            Plugin.Config.Save();
        }

        using var child = ImRaii.Child("duty-selection", new Vector2(-1f, -1f));
        if (!child.Success)
            return;

        var duties = Plugin.DataManager.GetExcelSheet<ContentFinderCondition>()
            .Where(cf => cf.Unknown47) // Unknown47 = IsInUse, is False for instances that aren't exist anymore
            .Where(cf => AllowedContentTypes.Contains(cf.ContentType.RowId));

        var searchQuery = DutySearchQuery.Trim();
        if (searchQuery.Trim() != "")
            duties = duties.Where(duty => duty.Name.ExtractText().ContainsIgnoreCase(searchQuery));

        foreach (var cf in duties)
        {
            var selected = filter.Duties.Contains(cf.RowId);
            var name = cf.Name.ExtractText();
            name = char.ToUpperInvariant(name[0]) + name[1..];
            if (!ImGui.Selectable(name, ref selected))
                continue;

            if (selected)
                filter.Duties.Add(cf.RowId);
            else
                filter.Duties.Remove(cf.RowId);

            Plugin.Config.Save();
        }
    }
}