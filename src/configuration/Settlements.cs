using livemap.layer.marker.options;
using livemap.layer.marker.options.type;
using Vintagestory.API.MathTools;
using Point = livemap.data.Point;

namespace livemap.configuration;

public class SettlementMetadata {
    public string Name { get; set; } = "Nameless Settlement";
    public Vec3i Location { get; set; } = new Vec3i(0, 0, 0);
    public string Description { get; set; } = "Here be monsters";
}

public class Settlements {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;

    public SettlementMetadata[] SettlementMeta { get; set; } = new SettlementMetadata[]{};

    public IconOptions IconOptions { get; set; } = new() {
        Title = "Settlements",
        Alt = "Settlements",
        IconUrl = "#svg-circle",
        IconSize = new Point(16, 16),
        Pane = "settlements"
    };

    public TooltipOptions? Tooltip { get; set; } = new() {
        Direction = "top",
        Content = "<em>{0}</em><br>{1}"
    };

    public PopupOptions? Popup { get; set; }

    public string? Css { get; set; } = ".leaflet-settlements-pane .leaflet-marker-icon{color:#D0006C;filter:drop-shadow(1px 0 0 black) drop-shadow(-1px 0 0 black) drop-shadow(0 1px 0 black) drop-shadow(0 -1px 0 black)}";
}
