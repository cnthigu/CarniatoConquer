using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Npcs
{
    public interface INpcDialogHandler
    {
        uint NpcId { get; }
        void HandleDialog(Character user, MsgTaskDialog msg, TQDialog dialog);
    }
}