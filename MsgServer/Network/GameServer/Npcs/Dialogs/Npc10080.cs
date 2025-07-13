using MsgServer.Structures.Entities;
using ServerCore.Common.Enums;
using ServerCore.Networking.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace MsgServer.Network.GameServer.Npcs.Dialogs
{
    public class Npc10080 : INpcDialogHandler
    {
        public uint NpcId => 10080;

        public void HandleDialog(Character user, MsgTaskDialog msg, TQDialog dialog)
        {
            dialog.SetAvatar(50);
            byte controlId = msg.OptionId;

            switch (controlId)
            {
                case 0:
                    dialog.AddText("Escolha uma Cidade para teleportar.");
                    dialog.AddOption("Desert City", 1);
                    dialog.AddOption("Phoenix Castle", 2);
                    dialog.AddOption("Ape Moutain", 3);
                    dialog.AddOption("Market ", 4);
                    dialog.Show();
                    break;

                case 1:

                    user.ChangeMoney(-1000);
                    user.ChangeMap(498, 652, 1000);
                    break;  

                case 2:
                    user.ChangeMoney(-1000);
                    user.ChangeMap(210, 260, 1011);
                    break;

                case 3:
                    user.ChangeMoney(-1000);
                    user.ChangeMap(566, 561, 1020);
                    break;

                case 4:
                    user.ChangeMoney(-1000);
                    user.ChangeMap(212, 195, 1036);
                    break;
            }
        }
    }
}
