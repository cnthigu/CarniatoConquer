using MsgServer.Network.GameServer.Npcs;
using MsgServer.Network.GameServer.Npcs.Dialogs;
using MsgServer.Structures.Entities;
using ServerCore.Networking.Packets;
using System;
using System.Collections.Generic;

namespace MsgServer.Network.GameServer.Npcs
{
    public static class NpcDialogRegistry
    {
        private static readonly Dictionary<uint, INpcDialogHandler> DialogHandlers = new Dictionary<uint, INpcDialogHandler>();

        static NpcDialogRegistry()
        {
            // Registrar NPCs específicos
            Console.WriteLine("Iniciando registro de NPCs específicos...");

            DialogHandlers.Add(16, new Npc16());
            DialogHandlers.Add(1209, new Npc1209());

            Console.WriteLine("Registrando Npc10080...");
            DialogHandlers.Add(10080, new Npc10080());
            Console.WriteLine("Npc10080 registrado.");

            // Registrar NPCs genéricos na faixa 60001–70000
            for (uint npcId = 60001; npcId <= 70000; npcId++)
            {
                if (!DialogHandlers.ContainsKey(npcId))
                {
                    DialogHandlers.Add(npcId, new GenericQuestNpc(npcId));
                }
            }
        }

        private static void RegisterHandler(INpcDialogHandler handler)
        {
            if (!DialogHandlers.ContainsKey(handler.NpcId))
            {
                DialogHandlers.Add(handler.NpcId, handler);
            }
        }

        public static bool HandleDialog(uint npcId, Character user, MsgTaskDialog msg, TQDialog dialog)
        {
            if (DialogHandlers.TryGetValue(npcId, out var handler))
            {
                handler.HandleDialog(user, msg, dialog);
                return true;
            }
            return false;
        }
    }
}