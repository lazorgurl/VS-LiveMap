using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using livemap.configuration;
using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.util;

namespace livemap.layer.builtin;

public class SpawnLayer : Layer {
    public override int? Interval => Config.UpdateInterval;

    public override bool? Hidden => !Config.DefaultShowLayer;

    public override string? Css => Config.Css;

    public override List<Marker> Markers {
        get {
            TooltipOptions? tooltip = Config.Tooltip?.DeepCopy();
            if (tooltip?.Content != null) {
                tooltip.Content = tooltip.Content;
            }
            PopupOptions? popup = Config.Popup?.DeepCopy();
            if (popup?.Content != null) {
                popup.Content = popup.Content;
            }
            return new List<Marker> {
                new Icon("livemap:spawn", LiveMap.Api.Sapi.World.DefaultSpawnPosition.ToPoint(), Config.IconOptions) {
                    Tooltip = tooltip,
                    Popup = popup
                }
            };
        }
    }

    public override string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    private static Spawn Config => LiveMap.Api.Config.Layers.Spawn;

    public SpawnLayer() : base("spawn", Config.IconOptions.Title ?? "Spawn") { }

    public override async Task WriteToDisk(CancellationToken cancellationToken) {
        if (Config.Enabled) {
            await base.WriteToDisk(cancellationToken);
        }
    }
}
