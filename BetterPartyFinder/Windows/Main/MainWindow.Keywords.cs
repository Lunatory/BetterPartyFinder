using System.Linq;
using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace BetterPartyFinder.Windows.Main;

public partial class MainWindow
{
    // private int SelectedWorld; //todo: make this into a whitelist/blacklist setter
    private bool WhitelistSelected = true;
    private string KeywordText = string.Empty;  

    // private int WhitelistModeALL = 0; // 0 = ANY, 1 = ALL

    private void DrawKeywordsTab(ConfigurationFilter filter)
    {
        ImGui.PushItemWidth(ImGui.GetWindowWidth() / 3f);
        ImGui.InputText("###keyword-text", ref KeywordText, 64);
        ImGui.SameLine();
        ImGui.Checkbox("###whitelist-selected", ref WhitelistSelected);
        ImGui.PopItemWidth();

        ImGui.SameLine();
        if (Helper.IconButton(FontAwesomeIcon.Plus, "add-keyword"))
        {
            var word = KeywordText.Trim();
            if (!string.IsNullOrEmpty(word))
            {
                filter.Keywords.Add(new KeywordInfo(word, WhitelistSelected));
                Plugin.Config.Save();
            }
        }

        ImGui.NewLine();
        DrawKeywordList("Whitelist", filter.Keywords.Where(k => k.Whitelist), filter);
        ImGui.NewLine();
        DrawKeywordList("Blacklist", filter.Keywords.Where(k => !k.Whitelist), filter);
    }

    private void DrawKeywordList(string label, IEnumerable<KeywordInfo> keywords, ConfigurationFilter filter)
    {
        // ImGui.TextUnformatted($"{label}:");
        if (label == "Whitelist")
        {
            ImGui.TextUnformatted($"Whitelist: {(filter.KeywordsMode ? "ALL" : "ANY")}");

            ImGui.SameLine();
            if (ImGui.Button("Toggle Mode"))
            {
                filter.KeywordsMode = !filter.KeywordsMode;
                Plugin.Config.Save();
            }
        }
        else
        {
            ImGui.TextUnformatted("Blacklist:");
        }   

        KeywordInfo? toDelete = null;

        foreach (var info in keywords)
        {
            ImGui.TextUnformatted(info.Word);
            ImGui.SameLine();
            if (Helper.IconButton(FontAwesomeIcon.Trash, $"delete-keyword-{info.GetHashCode()}"))
                toDelete = info;
        }

        if (toDelete != null)
        {
            filter.Keywords.Remove(toDelete);
            Plugin.Config.Save();
        }
    }
}