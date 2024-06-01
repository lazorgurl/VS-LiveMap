using System.Collections.Generic;
using System.IO;
using livemap.configuration;
using livemap.layer.marker;
using livemap.util;

namespace livemap.layer.builtin;

public class TranslocatorsLayer : Layer {
    public override int? Interval => Config.UpdateInterval;

    public override bool? Hidden => !Config.DefaultShowLayer;

    public override List<Marker> Markers => new() {
        //new Icon("livemap:spawn", LiveMap.Api.Sapi.World.DefaultSpawnPosition.ToPoint(), Config.IconOptions)
    };

    public override string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    private static Translocators Config => LiveMap.Api.Config.Layers.Translocators;

    public TranslocatorsLayer() : base("translocators", "lang.translocators".ToLang()) { }
}
