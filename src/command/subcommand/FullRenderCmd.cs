using System.Threading;
using livemap.data;
using Vintagestory.API.Common;
using Vintagestory.Common.Database;

namespace livemap.command.subcommand;

public class FullRenderCmd : AbstractCommand {
    public FullRenderCmd(LiveMap server) : base(server, new[] { "fullrender" }) { }

    public override CommandResult Execute(TextCommandCallingArgs args) {
        if (Server.Colormap.Count == 0) {
            return CommandResult.Error("fullrender.missing-colormap");
        }

        new Thread(_ => {
            // queue up all existing chunks
            foreach (ChunkPos chunk in new ChunkLoader(Server.Sapi).GetAllMapChunkPositions()) {
                Server.RenderTaskManager.Queue(chunk.X >> 4, chunk.Z >> 4);
            }

            // trigger world save to process the queue now
            Server.Sapi.Event.RegisterCallback(_ => {
                // do this back on the main thread
                Server.Sapi.ChatCommands.Get("autosavenow").Execute(args);
            }, 1);
        }).Start();

        return CommandResult.Success("fullrender.started");
    }
}
