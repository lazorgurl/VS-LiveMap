using livemap.layer.marker.options;
using livemap.layer.marker.options.type;
using Point = livemap.data.Point;

namespace livemap.configuration;

public class Translocators {
    public bool Enabled { get; set; } = true;

    public int UpdateInterval { get; set; } = 30;

    public bool DefaultShowLayer { get; set; } = false;

    public IconOptions IconOptions { get; set; } = new() {
        Title = "",
        Alt = "",
        IconUrl = "#svg-spiral",
        IconSize = new Point(16, 16),
        Pane = "translocators"
    };

    public TooltipOptions? Tooltip { get; set; } = new() {
        Direction = "top",
        Content = "{0}<br>{1}"
    };

    public PopupOptions? Popup { get; set; }

    public string? Css { get; set; } = ".leaflet-translocators-pane .leaflet-marker-icon{color:#204EA2;filter:drop-shadow(1px 0 0 black) drop-shadow(-1px 0 0 black) drop-shadow(0 1px 0 black) drop-shadow(0 -1px 0 black)} .leaflet-translocators-pane span{position:relative;top:-22px;display:block;text-align:center;color:black;filter: drop-shadow(1px 0 0 #a5b8d9) drop-shadow(-1px 0 0 #a5b8d9) drop-shadow(0 1px 0 #a5b8d9) drop-shadow(0 -1px 0 #a5b8d9)}";
}
