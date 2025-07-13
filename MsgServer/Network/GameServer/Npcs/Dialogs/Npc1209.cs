using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

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
                    dialog.AddText("testes to FoxConquer! This is a beta server when it is no longer beta you will continue to keep all the inventory and the level. Enjoy this server and report any bug you find :) ");
                    dialog.AddText("You receive 100 CPs, 50,000 silvers and 200 Study Points for staying 30 minutes online! ");
                    dialog.AddText("Can buy some required items for promotion here only talk me.");
                    dialog.AddOption("Buy Promotion Items", 1);
                    dialog.AddOption("Thanks", 255);
                    dialog.Show();
                    break;

                case 1:
                    dialog.AddText("Have this items for promotion you can buy.");
                    dialog.AddOption("Buy Emerald [250 CPs]", 2);
                    dialog.AddOption("Buy MoonBox [500 CPs]", 3);
                    dialog.AddOption("Buy 20 EuxeniteOre [2000 CPs]", 4);
                    dialog.AddOption("No thanks", 255);
                    dialog.Show();
                    break;

                case 2: // Emerald
                    if (user.ReduceEmoney(250))
                    {
                        if (user.Inventory.Create(1080001))
                        {
                            dialog.AddText("Here is your item!");
                            dialog.AddOption("Thanks", 255);
                        }
                        else
                        {
                            dialog.AddText("Not have space in your inventory");
                            dialog.AddOption("Ok", 255);
                        }
                    }
                    else
                    {
                        dialog.AddText("You not have 250 CPs.");
                        dialog.AddOption("Oh sorry", 255);
                    }
                    dialog.Show();
                    break;

                case 3: // MoonBox
                    if (user.ReduceEmoney(500))
                    {
                        if (user.Inventory.Create(721080))
                        {
                            dialog.AddText("Here is your item!");
                            dialog.AddOption("Thanks", 255);
                        }
                        else
                        {
                            dialog.AddText("Not have space in your inventory");
                            dialog.AddOption("Ok", 255);
                        }
                    }
                    else
                    {
                        dialog.AddText("You not have 500 CPs.");
                        dialog.AddOption("Oh sorry", 255);
                    }
                    dialog.Show();
                    break;

                case 4: // EuxeniteOre
                    if (user.ReduceEmoney(2000))
                    {
                        if (user.Inventory.Create(1072031, 20)) // Supondo que Create aceite quantidade
                        {
                            dialog.AddText("Here is your items!");
                            dialog.AddOption("Thanks", 255);
                        }
                        else
                        {
                            dialog.AddText("Not have space in your inventory");
                            dialog.AddOption("Ok", 255);
                        }
                    }
                    else
                    {
                        dialog.AddText("You not have 2000 CPs.");
                        dialog.AddOption("Oh sorry", 255);
                    }
                    dialog.Show();
                    break;
            }
        }
    }
}