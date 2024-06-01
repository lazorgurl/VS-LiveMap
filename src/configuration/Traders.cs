using JetBrains.Annotations;
using livemap.layer.marker.options;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

[PublicAPI]
public class Traders {
    public bool Enabled { get; set; } = false;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = true;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "",
        Alt = "",
        IconUrl = "#svg-trader",
        IconSize = new Point(16, 16),
        Pane = "traders"
    };

    public TooltipOptions? Tooltip { get; set; } = new() {
        Direction = "top",
        Content = "{0}<br>{1}"
    };

    public PopupOptions? Popup { get; set; }

    public string? Css { get; set; } = ".leaflet-traders-pane .leaflet-marker-icon { color: #204EA2; filter: drop-shadow(1px 0 0 black) drop-shadow(-1px 0 0 black) drop-shadow(0 1px 0 black) drop-shadow(0 -1px 0 black)}";
}
