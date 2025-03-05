using livemap.layer.marker.options;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

public class Translocators {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = false;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "Translocators",
        Alt = "translocators",
        IconUrl = "#svg-spiral",
        IconSize = new Point(16, 16),
        Pane = "translocators"
    };

    public TooltipOptions? Tooltip { get; set; } = new() {
        Direction = "top",
        Content = "{0}"
    };

    public PopupOptions? Popup { get; set; }

    public string? Css { get; set; } = ".leaflet-translocators-pane {color:##00FFF0;filter:drop-shadow(1px 0 0 black) drop-shadow(-1px 0 0 black) drop-shadow(0 1px 0 black) drop-shadow(0 -1px 0 black)}";
}
