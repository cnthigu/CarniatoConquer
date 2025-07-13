using DB.Entities;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;
using System;

namespace MsgServer.Network.GameServer.Npcs.Dialogs
{
    public class Npc1209 : INpcDialogHandler
    {
        public uint NpcId => 1209;

        public void HandleDialog(Character user, MsgTaskDialog msg, TQDialog dialog)
        {
            dialog.SetAvatar(82);
            byte controlId = msg.OptionId;

            switch (controlId)
            {
                case 0: 
                    dialog.AddText($"Ola, bem-vindo ao ConquerTANANA!\nSeu nivel VIP e: {user.VipLevel}.");
                    dialog.AddText($"\nVoce tem {user.DbUser.OnlinePoints} Online Points.");
                    dialog.AddText($"\nVoce ganha {(user.DbUser.MapId == 1002 ? 5 : 3)} Online Points a cada 5 minutos logado{(user.DbUser.MapId == 1002 ? " em Twin City" : " fora de Twin City")}.");
                    dialog.AddText("\nUse Online Points para subir seu nivel VIP!");
                    dialog.AddOption("Subir nivel VIP", 1);
                    dialog.AddOption("Nao, obrigado", 255);
                    dialog.Show();
                    break;

                case 1:
                    if (user.VipLevel >= 6)
                    {
                        dialog.AddText("Voce ja atingiu o nivel VIP maximo (VIP 6)!");
                        dialog.AddOption("Ok", 255);
                    }
                    else
                    {
                        int cost = (user.VipLevel + 1) * 1000; 
                        dialog.AddText($"Para subir para VIP {user.VipLevel + 1}, voce precisa de {cost} Online Points.");
                        dialog.AddOption($"Confirmar (VIP {user.VipLevel + 1})", 2);
                        dialog.AddOption("Nao, obrigado", 255);
                    }
                    dialog.Show();
                    break;

                case 2:
                    if (user.VipLevel >= 6)
                    {
                        dialog.AddText("Voce ja atingiu o nivel VIP maximo (VIP 6)!");
                        dialog.AddOption("Ok", 255);
                    }
                    else
                    {
                        int cost = (user.VipLevel + 1) * 1000;
                        if (user.ReduziOnlinePoints(cost))
                        {
                            user.VipLevel += 1; 
                            dialog.AddText($"Parabens! Voce agora e VIP {user.VipLevel}!");
                            dialog.AddOption("Obrigado", 255);
                            Console.WriteLine($"Jogador {user.Name} subiu para VIP {user.VipLevel}.");
                        }
                        else
                        {
                            dialog.AddText($"Voce nao tem {cost} Online Points para subir para VIP {user.VipLevel + 1}.");
                            dialog.AddOption("Desculpe-me", 255);
                        }
                    }
                    dialog.Show();
                    break;
            }
        }
    }
}