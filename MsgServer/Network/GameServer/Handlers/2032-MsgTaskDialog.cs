// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
//
// Computer User: Felipe Vendramini
// File Created by:  Felipe Vendramini
// zfserver v2.5517 - MsgServer - 2032 - MsgTaskDialog.cs
// Last Edit: 2016/12/27 15:29
// Created: 2016/12/07 00:27

using MsgServer.Structures;
using MsgServer.Structures.Entities;
using MsgServer.Structures.Interfaces;
using MsgServer.Structures.Items;
using ServerCore.Common;
using ServerCore.Networking.Packets;
using System.Linq;

// Adicione os namespaces para o novo sistema de NPC
using MsgServer.Network.GameServer.Npcs;
using MsgServer.Network.GameServer.Npcs.Dialogs;

namespace MsgServer.Network.GameServer.Handlers
{
    public static partial class Handlers
    {
        public static void HandleTaskDialog(Character pUser, MsgTaskDialog pMsg)
        {
            if (!pUser.IsAlive)
            {
                pUser.Send("You are dead.");
                return;
            }

            byte controlId = pMsg.OptionId;

            IScreenObject interactedNpc = null;
            if (pUser.Map.GameObjects.TryGetValue(pMsg.TaskId, out interactedNpc) || ServerKernel.Maps[5000].GameObjects.TryGetValue(pMsg.TaskId, out interactedNpc))
            {
                pUser.InteractingNpc = interactedNpc;
            }
            if (pUser.InteractingNpc != null)
            {
                TQDialog dialog = new TQDialog(pUser);

                if (NpcDialogRegistry.HandleDialog(pUser.InteractingNpc.Identity, pUser, pMsg, dialog))
                {
                    // Diálogo tratado por um handler de classe específico.
                    // A lógica antiga para este NPC não será executada, e a função termina aqui.
                    return;
                }

                #region Database NPC Dialogs

                switch (pMsg.InteractType)
                {

                    case MsgTaskDialog.MESSAGE_BOX:
                        {
                            if (pUser.CaptchaBox != null)
                            {
                                if (controlId == 0)
                                {
                                    pUser.CaptchaBox.OnCancel(pUser);
                                }
                                else
                                {
                                    pUser.CaptchaBox.OnOk(pUser);
                                }
                                pUser.CaptchaBox = null;
                            }
                            else if (pUser.RequestBox != null)
                            {
                                if (controlId == 0) // cancel
                                {
                                    pUser.RequestBox.OnCancel(pUser);
                                }
                                else if (controlId == 255) // ok
                                {
                                    pUser.RequestBox.OnOk(pUser);
                                }
                                pUser.RequestBox = null;
                            }
                            break;
                        }
                    // This is what we receive when we click on OK of a Input box on a NPC
                    // Or a option of the NPC.
                    case MsgTaskDialog.ANSWER:
                    case MsgTaskDialog.TEXT_INPUT:
                        {
                            if (pMsg.InteractType == 102)
                            {
                                if (pUser.SyndicateIdentity <= 0)
                                    return;

                                pUser.Syndicate.ExpelMember(pUser, pMsg.Text, true);
                                return;
                            }

                            if (controlId == 255)
                                break;

                            if (pUser.NextActions.TryGetValue(controlId, out INextAction action))
                            {
                                pUser.NextActions.Clear();
                                if (pUser.InteractingNpc != null &&
                                    (pUser.InteractingNpc.MapIdentity == 5000 ||
                                     Calculations.InScreen(pUser.MapX, pUser.MapY,
                                         pUser.InteractingNpc.MapX, pUser.InteractingNpc.MapY)))
                                {
                                    if (pUser.InteractingNpc is GameNpc)
                                    {
                                        var pNpc = pUser.InteractingNpc as GameNpc;
                                        pUser.GameAction.ProcessAction(GetNextAction(pUser, action.Identity), pUser, pNpc, null,
                                            action.IsInput ? pMsg.Text : null);
                                    }
                                    else if (pUser.InteractingNpc is DynamicNpc)
                                    {
                                        var pNpc = pUser.InteractingNpc as DynamicNpc;
                                        pUser.GameAction.ProcessAction(GetNextAction(pUser, action.Identity), pUser, pNpc, null,
                                            action.IsInput ? pMsg.Text : null);
                                    }
                                }
                                else if (pUser.TaskItem != null)
                                {
                                    pUser.GameAction.ProcessAction(GetNextAction(pUser, action.Identity), pUser, null, pUser.TaskItem,
                                        action.IsInput ? pMsg.Text : null);
                                    pUser.TaskItem = null;
                                }
                            }
                            break;
                        }
                    default:
                        {
                            switch (controlId)
                            {
                                case 0:
                                    {
                                        pUser.NextActions.Clear();
                                        if (pUser.Map.GameObjects.TryGetValue(pMsg.TaskId, out IScreenObject pNpc)
                                            || ServerKernel.Maps[5000].GameObjects.TryGetValue(pMsg.TaskId, out pNpc))
                                        {
                                            pUser.GameAction.ProcessAction(GetActionIdentity(pUser, pNpc), pUser, pNpc, null,
                                                "");
                                        }
                                        break;
                                    }
                                case 255:
                                    {
                                        // Close Dialog
                                        pUser.TaskItem = null;
                                        pUser.InteractingNpc = null;
                                        pUser.NextActions.Clear();
                                        break;
                                    }
                                default:
                                    {
                                        pUser.TaskItem = null;
                                        pUser.InteractingNpc = null;
                                        pUser.NextActions.Clear();
                                        ServerKernel.Log.SaveLog(string.Format("Npc interact type default [{0}] not handled.", controlId), false, LogType.WARNING);
                                        break;
                                    }
                            }
                            break;
                        }
                }
                #endregion
            }
            else if (pUser.TaskItem != null)
            {
                switch (pMsg.InteractType)
                {
                    case MsgTaskDialog.ANSWER:
                        {
                            if (pUser.NextActions.TryGetValue(pMsg.OptionId, out INextAction action))
                            {
                                pUser.NextActions.Clear();
                                pUser.GameAction.ProcessAction(action.Identity, pUser, null, pUser.TaskItem, action.IsInput ? pMsg.Text : null);
                            }
                            break;
                        }
                    case 255:
                        {
                            // Close Dialog
                            pUser.TaskItem = null;
                            pUser.InteractingNpc = null;
                            pUser.NextActions.Clear();
                            break;
                        }
                    default:
                        {
                            pUser.TaskItem = null;
                            pUser.InteractingNpc = null;
                            pUser.NextActions.Clear();
                            ServerKernel.Log.SaveLog(string.Format("Npc interact type default [{0}] not handled for item.", controlId), false, LogType.WARNING);
                            break;
                        }
                }
            }
        }

        private static uint GetNextAction(Character pUser, uint idAction)
        {
            if (ServerKernel.GameTasks.TryGetValue(idAction, out TaskStruct pTask))
            {
                bool bItem1 = true,
                    bItem2 = true,
                    bMoney = true,
                    bProf = true,
                    bSex = true,
                    bMinPk = true,
                    bMaxPk = true,
                    bTeam = true,
                    bMetem = true,
                    bMarriage = true;
                if (pTask.Itemname1 != "")
                    bItem1 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname1) != null;
                if (pTask.Itemname2 != "")
                    bItem2 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname2) != null;
                if (pTask.Money > 0)
                    bMoney = pUser.Silver >= pTask.Money;
                if (pTask.Profession > 0)
                {
                    if (pTask.Profession > 9)
                        bProf = pUser.Profession / 10 == pTask.Profession;
                    else
                        bProf = pUser.Profession == pTask.Profession;
                }
                if (pTask.Sex > 0 && pTask.Sex < 999)
                    bSex = pUser.Gender == pTask.Sex;
                bMinPk = pUser.PkPoints >= pTask.MinPk;
                bMaxPk = pUser.PkPoints <= pTask.MaxPk;
                if (pTask.Team != 0 && pTask.Team != 999)
                    bTeam = pUser.Team != null;
                if (pTask.Metempsychosis > 0)
                    bMetem = pUser.Metempsychosis >= pTask.Metempsychosis;
                if (pTask.Marriage > 0)
                    bMarriage = pUser.Mate != "None";

                bool bRet = bItem1 && bItem2 && bMoney && bProf && bSex && bMinPk
                                && bMaxPk && bTeam && bMetem && bMarriage;

                if (bRet)
                {
                    idAction = pTask.IdNext;
                }
                if (!bRet && pTask.IdNextfail != 0)
                {
                    idAction = pTask.IdNextfail;
                }
            }
            return idAction;
        }

        private static uint GetActionIdentity(Character pUser, IScreenObject pNpc)
        {
            uint idAction = 0;
            if (pNpc is GameNpc)
            {
                GameNpc pGameNpc = pNpc as GameNpc;

                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0: idAction = pGameNpc.Task0; break;
                        case 1: idAction = pGameNpc.Task1; break;
                        case 2: idAction = pGameNpc.Task2; break;
                        case 3: idAction = pGameNpc.Task3; break;
                        case 4: idAction = pGameNpc.Task4; break;
                        case 5: idAction = pGameNpc.Task5; break;
                        case 6: idAction = pGameNpc.Task6; break;
                        case 7: idAction = pGameNpc.Task7; break;
                    }

                    if (ServerKernel.GameTasks.TryGetValue(idAction, out TaskStruct pTask))
                    {
                        bool bItem1 = true,
                            bItem2 = true,
                            bMoney = true,
                            bProf = true,
                            bSex = true,
                            bMinPk = true,
                            bMaxPk = true,
                            bTeam = true,
                            bMetem = true,
                            bMarriage = true;
                        if (pTask.Itemname1 != "")
                            bItem1 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname1) != null;
                        if (pTask.Itemname2 != "")
                            bItem2 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname2) != null;
                        if (pTask.Money > 0)
                            bMoney = pUser.Silver >= pTask.Money;
                        if (pTask.Profession > 0)
                        {
                            if (pTask.Profession > 9)
                                bProf = pUser.Profession / 10 == pTask.Profession;
                            else
                                bProf = pUser.Profession == pTask.Profession;
                        }
                        if (pTask.Sex > 0 && pTask.Sex < 999)
                            bSex = pUser.Gender == pTask.Sex;
                        bMinPk = pUser.PkPoints >= pTask.MinPk;
                        bMaxPk = pUser.PkPoints <= pTask.MaxPk;
                        if (pTask.Team != 0 && pTask.Team != 999)
                            bTeam = pUser.Team != null;
                        if (pTask.Metempsychosis > 0)
                            bMetem = pUser.Metempsychosis >= pTask.Metempsychosis;
                        if (pTask.Marriage > 0)
                            bMarriage = pUser.Mate != "None";

                        bool bRet = bItem1 && bItem2 && bMoney && bProf && bSex && bMinPk
                                        && bMaxPk && bTeam && bMetem && bMarriage;

                        if (bRet)
                        {
                            idAction = pTask.IdNext;
                            break;
                        }
                        if (!bRet && pTask.IdNextfail != 0)
                        {
                            idAction = pTask.IdNextfail;
                            break;
                        }
                    }
                }
            }
            else if (pNpc is DynamicNpc)
            {
                DynamicNpc pGameNpc = pNpc as DynamicNpc;

                for (int i = 0; i < 8; i++)
                {
                    switch (i)
                    {
                        case 0: idAction = pGameNpc.Task0; break;
                        case 1: idAction = pGameNpc.Task1; break;
                        case 2: idAction = pGameNpc.Task2; break;
                        case 3: idAction = pGameNpc.Task3; break;
                        case 4: idAction = pGameNpc.Task4; break;
                        case 5: idAction = pGameNpc.Task5; break;
                        case 6: idAction = pGameNpc.Task6; break;
                        case 7: idAction = pGameNpc.Task7; break;
                    }

                    if (ServerKernel.GameTasks.TryGetValue(idAction, out TaskStruct pTask))
                    {
                        bool bItem1 = true,
                            bItem2 = true,
                            bMoney = true,
                            bProf = true,
                            bSex = true,
                            bMinPk = true,
                            bMaxPk = true,
                            bTeam = true,
                            bMetem = true,
                            bMarriage = true;
                        if (pTask.Itemname1 != "")
                            bItem1 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname1) != null;
                        if (pTask.Itemname2 != "")
                            bItem2 = pUser.Inventory.Items.Values.FirstOrDefault(x => x.Itemtype.Name == pTask.Itemname2) != null;
                        if (pTask.Money > 0)
                            bMoney = pUser.Silver >= pTask.Money;
                        if (pTask.Profession > 0)
                        {
                            if (pTask.Profession > 9)
                                bProf = pUser.Profession / 10 == pTask.Profession;
                            else
                                bProf = pUser.Profession == pTask.Profession;
                        }
                        if (pTask.Sex > 0 && pTask.Sex < 999)
                            bSex = pUser.Gender == pTask.Sex;
                        bMinPk = pUser.PkPoints >= pTask.MinPk;
                        bMaxPk = pUser.PkPoints <= pTask.MaxPk;
                        if (pTask.Team != 0 && pTask.Team != 999)
                            bTeam = pUser.Team != null;
                        if (pTask.Metempsychosis > 0)
                            bMetem = pUser.Metempsychosis >= pTask.Metempsychosis;
                        if (pTask.Marriage > 0)
                            bMarriage = pUser.Mate != "None";

                        bool bRet = bItem1 && bItem2 && bMoney && bProf && bSex && bMinPk
                                        && bMaxPk && bTeam && bMetem && bMarriage;

                        if (bRet)
                        {
                            idAction = pTask.IdNext;
                            break;
                        }
                        if (!bRet && pTask.IdNextfail != 0)
                        {
                            idAction = pTask.IdNextfail;
                            break;
                        }
                    }
                }
            }
            return idAction;
        }
    }
}
