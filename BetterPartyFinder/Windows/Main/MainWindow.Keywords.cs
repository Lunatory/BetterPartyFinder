using System.Linq;
using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    private bool WhitelistSelected = true;
    private string KeywordText = string.Empty;  

    private void DrawKeywordsTab(ConfigurationFilter filter)
    {
        using (ImRaii.ItemWidth(ImGui.GetWindowWidth() * 0.65f))
            ImGui.InputText("###keyword-text", ref KeywordText, 64);
        ImGui.SameLine();
        ImGui.Checkbox("###whitelist-selected", ref WhitelistSelected);
        if (ImGui.IsItemHovered()) 
            ImGui.SetTooltip("Checked = add to whitelist\nUnchecked = add to blacklist");

        ImGui.SameLine();
        if (Helper.IconButton(FontAwesomeIcon.Plus, "add-keyword"))
        {
            var word = KeywordText.Trim();
            if (word != string.Empty)
            {
                if (WhitelistSelected) {filter.Keywords.Whitelist.Add(word);}
                else {filter.Keywords.Blacklist.Add(word);}
                Plugin.Config.Save();
            }
        }

        ImGui.NewLine();
        DrawKeywordList("Whitelist", filter.Keywords.Whitelist, filter);
        ImGui.NewLine();
        DrawKeywordList("Blacklist", filter.Keywords.Blacklist, filter);
    }

    private void DrawKeywordList(string label, List<string> keywords, ConfigurationFilter filter)
    {
        if (label == "Whitelist")
        {
            ImGui.TextUnformatted($"Whitelist: {(filter.Keywords.WLMode == WhitelistMode.All ? "ALL" : "ANY")}");

            ImGui.SameLine();
            if (ImGui.Button("Toggle Mode"))
            {
                filter.Keywords.WLMode = filter.Keywords.WLMode == WhitelistMode.All ? WhitelistMode.Any : WhitelistMode.All; // toggle between ALL and ANY
                Plugin.Config.Save();
            }
        }
        else
        {
            ImGui.TextUnformatted("Blacklist:");
        }

        string toDelete = string.Empty;

        foreach (var word in keywords)
        {
            ImGui.TextUnformatted(word);
            ImGui.SameLine();
            if (Helper.IconButton(FontAwesomeIcon.Trash, $"delete-keyword-{word.GetHashCode()}"))
                toDelete = word;
        }

        if (toDelete != string.Empty)
        {
            keywords.Remove(toDelete);
            Plugin.Config.Save();
        }
    }
}