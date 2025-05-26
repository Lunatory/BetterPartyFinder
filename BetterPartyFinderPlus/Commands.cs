﻿using System;
using System.Collections.Generic;
using Dalamud.Game.Command;

namespace BetterPartyFinderPlus;

public class Commands : IDisposable {
    private static readonly Dictionary<string, string> CommandNames = new()
    {
        ["/betterpartyfinder"] = "Opens the main interface. Use with args \"c\" or \"config\" to open the settings.",
        ["/bpf"] = "Alias for /betterpartyfinder",
    };

    private Plugin Plugin { get; }

    internal Commands(Plugin plugin) {
        Plugin = plugin;

        foreach (var (name, help) in CommandNames)
            Plugin.CommandManager.AddHandler(name, new CommandInfo(OnCommand) { HelpMessage = help, });
    }

    public void Dispose()
    {
        foreach (var name in CommandNames.Keys)
            Plugin.CommandManager.RemoveHandler(name);
    }

    private void OnCommand(string command, string args)
    {
        if (args is "c" or "config")
            Plugin.ConfigWindow.Toggle();
        else
            Plugin.MainWindow.Toggle();
    }
}