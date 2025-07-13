using MsgServer.Network.GameServer.Npcs;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;

namespace MsgServer.Network.GameServer.Npcs.Dialogs
{
    public class Npc16 : INpcDialogHandler
    {
        public uint NpcId => 16;

        public void HandleDialog(Character user, MsgTaskDialog msg, TQDialog dialog)
        {
            dialog.SetAvatar(50);
            byte controlId = msg.OptionId;

            switch (controlId)
            {
                case 0:
                    dialog.AddText($"Hello, {user.Name}! Welcome to my shop in Twin City. I sell rare items!");
                    dialog.AddOption("See your items", 1);
                    dialog.AddOption("Goodbye", 255);
                    dialog.Show();
                    break;
                case 1:
                    dialog.AddText("Here are the items I have for sale:");
                    dialog.AddOption("Buy DragonBall [1000 CPs]", 2);
                    dialog.AddOption("Buy Meteor [500 CPs]", 3);
                    dialog.AddOption("No thanks", 255);
                    dialog.Show();
                    break;
                case 2:
                    if (user.ReduceEmoney(1000))
                    {
                        if (user.Inventory.Create(1088001))
                        {
                            dialog.AddText("Here is your DragonBall!");
                            dialog.AddOption("Thanks!", 255);
                        }
                        else
                        {
                            dialog.AddText("You don't have enough space in your inventory.");
                            dialog.AddOption("Ok", 255);
                        }
                    }
                    else
                    {
                        dialog.AddText("You don't have 1000 CPs.");
                        dialog.AddOption("Oh, sorry", 255);
                    }
                    dialog.Show();
                    break;
                case 3:
                    if (user.ReduceEmoney(500))
                    {
                        if (user.Inventory.Create(1081001))
                        {
                            dialog.AddText("Here is your Meteor!");
                            dialog.AddOption("Thanks!", 255);
                        }
                        else
                        {
                            dialog.AddText("You don't have enough space in your inventory.");
                            dialog.AddOption("Ok", 255);
                        }
                    }
                    else
                    {
                        dialog.AddText("You don't have 500 CPs.");
                        dialog.AddOption("Oh, sorry", 255);
                    }
                    dialog.Show();
                    break;
            }
        }
    }
}