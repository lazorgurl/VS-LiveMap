using livemap.layer;
using livemap.layer.builtin;

namespace livemap.registry;

public class LayerRegistry : Registry<Layer> {
    public PlayersLayer? Players { get; private set; }
    public SpawnLayer? Spawn { get; private set; }
    public TradersLayer? Traders { get; private set; }
    public TranslocatorsLayer? Translocators { get; private set; }
    public SettlementsLayer? Settlements { get; private set; }

    public LayerRegistry() : base("layers") { }

    public void RegisterBuiltIns() {
        Register(Players = new PlayersLayer());
        Register(Spawn = new SpawnLayer());
        Register(Traders = new TradersLayer());
        Register(Translocators = new TranslocatorsLayer());
        Register(Settlements = new SettlementsLayer());
    }

    public override void Dispose() {
        Players = null;
        Spawn = null;
        Traders = null;
        Translocators = null;
        Settlements = null;

        base.Dispose();
    }
}
