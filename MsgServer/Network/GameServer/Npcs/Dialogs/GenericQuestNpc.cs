using MsgServer.Network.GameServer.Npcs;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Npcs.Dialogs
{
    public class GenericQuestNpc : INpcDialogHandler
    {
        private readonly uint _npcId;

        public GenericQuestNpc(uint npcId)
        {
            _npcId = npcId;
        }

        public uint NpcId => _npcId;

        public void HandleDialog(Character user, MsgTaskDialog msg, TQDialog dialog)
        {
            dialog.SetAvatar(1);
            byte controlId = msg.OptionId;

            switch (controlId)
            {
                case 0:
                    dialog.AddText($"Hello, I am NPC {_npcId}. I don't have a specific dialog yet!");
                    dialog.AddOption("Okay", 255);
                    dialog.Show();
                    break;
            }
        }
    }
}