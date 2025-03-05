using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using livemap.configuration;
using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.util;
using Vintagestory.API.Util;

namespace livemap.layer.builtin;

public class SettlementsLayer : Layer {
    public override int? Interval => Config.UpdateInterval;

    public override bool? Hidden => !Config.DefaultShowLayer;

    public override List<Marker> Markers {
        get {
            List<Marker> list = new();
            Config.SettlementMeta.Foreach(meta => {
                TooltipOptions? tooltip = Config.Tooltip?.DeepCopy();
                if (tooltip?.Content != null) {
                    tooltip.Content = string.Format(tooltip.Content, meta.Name, meta.Description);
                }
                PopupOptions? popup = Config.Popup?.DeepCopy();
                if (popup?.Content != null) {
                    popup.Content = string.Format(popup.Content, meta.Name, meta.Description);
                }
                list.Add(new Icon($"settlement:{meta.Name}{meta.Location}", meta.Location.ToPoint().Add(LiveMap.Api.Sapi.World.DefaultSpawnPosition.ToPoint()), Config.IconOptions) {
                    Tooltip = tooltip,
                    Popup = popup
                });
            });
            return list;
        }
    }

    public override string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    public override string? Css => Config.Css;

    private static Settlements Config => LiveMap.Api.Config.Layers.Settlements;

    public SettlementsLayer() : base("settlements", Config.IconOptions.Title ?? "Settlements") { }

    public override async Task WriteToDisk(CancellationToken cancellationToken) {
        if (Config.Enabled) {
            await base.WriteToDisk(cancellationToken);
        }
    }

}
