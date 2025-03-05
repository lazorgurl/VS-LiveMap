using livemap.layer.marker.options;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

public class Traders {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = false;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "Traders",
        Alt = "traders",
        IconUrl = "#svg-trader",
        IconSize = new Point(16, 16),
        Pane = "traders"
    };

    public TooltipOptions? Tooltip { get; set; } = new() {
        Direction = "top",
        Content = "{0}<br>{1}"
    };

    public PopupOptions? Popup { get; set; }

    public string? Css { get; set; } = ".leaflet-traders-pane {color:#FF7518;filter:drop-shadow(1px 0 0 black) drop-shadow(-1px 0 0 black) drop-shadow(0 1px 0 black) drop-shadow(0 -1px 0 black)}";
}

