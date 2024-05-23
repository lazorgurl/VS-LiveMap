using System;
using System.Collections.Generic;
using ConfigLib;
using livemap.gui.settings;
using livemap.logger;
using livemap.util;
using Vintagestory.API.Server;

namespace livemap.gui;

public class ConfigGui : Gui {
    private readonly List<Gui> _guis = new();

    public ConfigGui(LiveMapClient client) : base(client) {
        ConfigLibModSystem? configlib = _client.Api.ModLoader.GetModSystem<ConfigLibModSystem>();
        configlib.ConfigWindowOpened += OnOpen;
        configlib.ConfigWindowClosed += OnClose;
        configlib.RegisterCustomConfig(client.ModId, (_, controlButtons) => {
            try {
                return Draw(controlButtons);
            } catch (Exception) {
                return new ControlButtons(false);
            }
        });

        _guis.Add(new ColormapSettings(client));
        _guis.Add(new HttpdSettings(client));
        _guis.Add(new WebSettings(client));
        _guis.Add(new ZoomSettings(client));
    }

    public override void OnOpen() {
        if (_client.Api.World.Player.HasPrivilege(Privilege.root)) {
            _client.RequestConfig();
        }
        _guis.ForEach(gui => {
            try {
                gui.OnOpen();
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        });
    }

    public override void OnClose() {
        _guis.ForEach(gui => {
            try {
                gui.OnClose();
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        });
    }

    public override void Draw() {
        _guis.ForEach(gui => {
            try {
                gui.Draw();
            } catch (Exception e) {
                Logger.Error(e.ToString());
            }
        });
    }

    public void Dispose() {
        _guis.Clear();
    }

    private ControlButtons Draw(ControlButtons controlButtons) {
        if (!_client.Api.World.Player.HasPrivilege(Privilege.root)) {
            Text($"{"access-denied".ToLang()}", true, 0xFFFF4040);
            return new ControlButtons(false);
        }

        if (_client.Config == null) {
            Text($"{"no-data-to-display".ToLang()}", true, 0xFFFF4040);
            _client.RequestConfig();
            return new ControlButtons(false);
        }

        if (controlButtons.Save) {
            // saves changes to config
            // todo send values back to server
            Logger.Info("SAVE");
            _client.SendConfig();
        }

        if (controlButtons.Restore) {
            // retrieves values from config
            // todo request new values from server
            Logger.Info("RESTORE");
        }

        if (controlButtons.Reload) {
            // applies settings changes
            // todo not needed
            Logger.Info("RELOAD");
        }

        if (controlButtons.Defaults) {
            // sets settings to default values
            // todo do we want original defaults or current defaults?
            Logger.Info("DEFAULTS");
        }

        Draw();

        return new ControlButtons(true) { Reload = false };
    }
}
