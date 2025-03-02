using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using livemap.configuration;
using livemap.data;
using livemap.layer.marker;
using livemap.layer.marker.options;
using livemap.util;
using Newtonsoft.Json;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace livemap.layer.builtin;

public class TranslocatorsLayer : Layer {
    public override int? Interval => Config.UpdateInterval;

    public override bool? Hidden => !Config.DefaultShowLayer;

    public override List<Marker> Markers {
        get {
            List<Marker> list = new();
            _knownTranslocators.Values.Foreach(translocators => {
                translocators.Foreach(translocator => {
                    TooltipOptions? tooltip = Config.Tooltip?.DeepCopy();
                    if (tooltip?.Content != null) {
                        tooltip.Content = string.Format(tooltip.Content, translocator.Name, $"to {translocator.To.ToPoint().X}");
                    }

                    PopupOptions? popup = Config.Popup?.DeepCopy();
                    if (popup?.Content != null) {
                        popup.Content = string.Format(popup.Content, translocator.Name, $"to {translocator.To.ToPoint()}");
                    }

                    list.Add(new Icon($"translocator:{translocator.Pos}", translocator.Pos.ToPoint(), Config.IconOptions) {
                        Tooltip = tooltip,
                        Popup = popup
                    });
                    list.Add(new Polyline($"translocator-line:{translocator.Pos}", new[] { translocator.Pos.ToPoint(), translocator.To.ToPoint() }));
                });
            });
            return list;
        }
    }

    public override string? Css => Config.Css;

    public override string Filename => Path.Combine(Files.MarkerDir, $"{Id}.json");

    private static Translocators Config => LiveMap.Api.Config.Layers.Translocators;

    private readonly ConcurrentDictionary<ulong, HashSet<Translocator>> _knownTranslocators;
    private readonly string _knownFile;

    public static readonly string[] TranslocatorNameAdjectives = new[] {
        "eldritch", "cyclopean", "non-euclidean", "primordial", "abyssal", "blasphemous", "antediluvian",
        "gibbous", "stygian", "chthonic", "accursed", "nameless", "forgotten", "ancient", "sunken", "foetid",
        "miasmic", "unspeakable", "unfathomable", "decrepit", "sepulchral", "ghastly", "tenebrous",
        "malevolent", "squamous", "rugose", "eldrich", "ichorish", "decadent", "writhing", "fungal",
        "vermiculate", "aberrant", "antique", "maddening", "profane", "betentacled", "sanguine", "feverish",
        "pallid", "celestial", "crepuscular", "lurking", "fetid", "slimy", "otherworldly", "unnatural",
        "sibilant", "preternatural", "vestigial"
    };

    public static readonly string[] TranslocatorNameNouns = new[] {
        "mountain", "valley", "plateau", "canyon", "peninsula", "isthmus", "archipelago", "fjord",
        "delta", "mesa", "volcano", "reef", "lagoon", "glacier", "tundra", "prairie", "savanna",
        "oasis", "gorge", "waterfall", "cave", "basin", "strait", "harbor", "atoll", "plain",
        "cliff", "dune", "forest", "river", "lake", "ocean", "island", "swamp", "marsh", "estuary",
        "geyser", "butte", "cape", "ravine", "ridge", "hill", "cove", "bay", "desert", "steppe",
        "rainforest", "jungle", "wetland", "escarpment"
    };

    private bool _dirty;

    public TranslocatorsLayer() : base("translocators", "lang.translocators".ToLang()) {
        _knownFile = Path.Combine(Files.JsonDir, $"{Id}.json");
        ConcurrentDictionary<ulong, HashSet<Translocator>>? translocators = null;
        if (File.Exists(_knownFile)) {
            try {
                string json = File.ReadAllText(_knownFile);
                translocators = JsonConvert.DeserializeObject<ConcurrentDictionary<ulong, HashSet<Translocator>>>(json);
            }
            catch (Exception) {
                // ignored
            }
        }

        _knownTranslocators = translocators ?? new ConcurrentDictionary<ulong, HashSet<Translocator>>();
    }

    public void SetTranslocators(ulong chunkIndex, HashSet<Translocator> translocators) {
        if (translocators.Count == 0) {
            _knownTranslocators.Remove(chunkIndex);
        }
        else {
            _knownTranslocators[chunkIndex] = translocators;
        }

        _dirty = true;
    }

    public override async Task WriteToDisk(CancellationToken cancellationToken) {
        if (_dirty) {
            string knownJson = JsonConvert.SerializeObject(_knownTranslocators, Files.JsonSerializerMinifiedSettings);

            if (cancellationToken.IsCancellationRequested) {
                return;
            }

            await Files.WriteJsonAsync(_knownFile, knownJson, cancellationToken);

            if (cancellationToken.IsCancellationRequested) {
                return;
            }
        }

        await base.WriteToDisk(cancellationToken);
    }

    public static string GenerateTranslocatorName(BlockPos pos, BlockPos to) {
        string seed = pos.ToString() + to.ToString();
        Random rng = new(seed.GetHashCode());
        Int64 adj = rng.NextInt64(50);
        Int64 noun = rng.NextInt64(50);

        return TranslocatorNameAdjectives[adj] + " " + TranslocatorNameNouns[noun];
    }

    public class Translocator {
        public readonly string Name;
        public readonly BlockPos Pos;
        public readonly BlockPos To;

        public Translocator(string name, BlockPos pos, BlockPos to) {
            Name = name;
            To = to;
            Pos = pos;
        }
    }
}
