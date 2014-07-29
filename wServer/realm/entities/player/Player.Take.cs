#region

using System;
using System.Collections.Generic;
using System.IO;
using wServer.cliPackets;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        private bool intake;

        public void RequestTake(RealmTime time, RequestTradePacket pkt)
        {
            if (tradeTarget != null)
            {
                SendError("You're already trading!");
                tradeTarget = null;
                return;
            }
            Player target = Owner.GetUniqueNamedPlayer(pkt.Name);
            if (target == null)
            {
                SendError("Player not found!");
                return;
            }
            if (target.tradeTarget != null && target.tradeTarget != this)
            {
                SendError(target.Name + " is already trading!");
                return;
            }
            tradeTarget = target;
            trade = new bool[12];
            tradeAccepted = false;
            target.tradeTarget = this;
            target.trade = new bool[12];
            taking = true;
            tradeTarget.intake = true;
            var my = new TradeItem[Inventory.Length];
            for (int i = 0; i < Inventory.Length; i++)
                my[i] = new TradeItem
                {
                    Item = target.Inventory[i] == null ? -1 : target.Inventory[i].ObjectType,
                    SlotType = target.SlotTypes[i],
                    Included = false,
                    Tradeable = (target.Inventory[i] != null)
                };
            var your = new TradeItem[target.Inventory.Length];
            for (int i = 0; i < target.Inventory.Length; i++)
                your[i] = new TradeItem
                {
                    Item = -1,
                    SlotType = target.SlotTypes[i],
                    Included = false,
                    Tradeable = false
                };

            psr.SendPacket(new TradeStartPacket
            {
                MyItems = my,
                YourName = target.Name,
                YourItems = your
            });
        }

        private void TakeTick(RealmTime time)
        {
            if (trade != null)
                if (taking)
                    if (tradeTarget != null)
                        if (tradeAccepted)
                            if (tradeTarget != null && Owner != null && tradeTarget.Owner != null &&
                                Owner == tradeTarget.Owner)
                            {
                                name1 = Name;
                                name2 = tradeTarget.Name;
                                DoTake();
                            }
                            else
                            {
                                tradeTarget.tradeTarget = null;
                                tradeTarget.trade = null;
                                tradeTarget.intake = false;
                                tradeTarget.tradeAccepted = false;
                                tradeTarget = null;
                                trade = null;
                                taking = false;
                                tradeAccepted = false;
                            }
        }

        private void DoTake()
        {
            if (tradeTarget != null && Owner != null && tradeTarget.Owner != null && Owner == tradeTarget.Owner)
            {
                var thisItems = new List<Item>();
                for (int i = 0; i < trade.Length; i++)
                    if (trade[i])
                    {
                        if (tradeTarget.Inventory[i] != null)
                        {
                            thisItems.Add(tradeTarget.Inventory[i]);
                            tradeTarget.Inventory[i] = null;
                            UpdateCount++;
                        }
                        if (itemnumber1 == 0)
                        {
                            items1 = items1 + " " + thisItems[itemnumber1].ObjectId;
                        }
                        else if (itemnumber1 > 0)
                        {
                            items1 = items1 + ", " + thisItems[itemnumber1].ObjectId;
                        }
                        itemnumber1++;
                    }
                if (thisItems.Count == 0)
                    thisItems.Add(null);
                for (int i = 0; i < Inventory.Length; i++) //put items by slotType
                    if (Inventory[i] == null)
                    {
                        if (SlotTypes[i] == 10 && thisItems[0] != null)
                        {
                            Inventory[i] = thisItems[0];
                            thisItems.RemoveAt(0);
                        }
                        else
                        {
                            int itmIdx = -1;
                            for (int j = 0; j < thisItems.Count; j++)
                            {
                                try
                                {
                                    if (thisItems[j].SlotType == SlotTypes[i])
                                    {
                                        itmIdx = j;
                                        break;
                                    }
                                }
                                catch
                                {
                                    itmIdx = -1;
                                }
                            }
                            if (itmIdx != -1)
                            {
                                Inventory[i] = thisItems[itmIdx];
                                thisItems.RemoveAt(itmIdx);
                            }
                        }
                        if (thisItems.Count == 0) break;
                    }
                if (thisItems.Count > 0)
                    for (int i = 0; i < Inventory.Length; i++) //force put item
                        if (Inventory[i] == null)
                        {
                            Inventory[i] = thisItems[0];
                            thisItems.RemoveAt(0);
                            if (thisItems.Count == 0) break;
                        }
                psr.SendPacket(new TradeDonePacket
                {
                    Result = 1,
                    Message = "Items Taken"
                });

                const string dir = @"logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                using (var writer = new StreamWriter(@"logs\TakeLog.log", true))
                {
                    writer.WriteLine(Name + " took " + "{" + items1 + "}" + " from " + tradeTarget.Name);
                }
                Console.Out.WriteLine(Name + " took " + "{" + items1 + "}" + " from " + tradeTarget.Name);
                items1 = "";
                items2 = "";
                itemnumber1 = 0;
                itemnumber2 = 0;
                UpdateCount++;
                tradeTarget.UpdateCount++;
                name1 = "";
                name2 = "";
                tradeTarget.intake = false;
                tradeTarget.tradeTarget = null;
                tradeTarget.trade = null;
                tradeTarget.tradeAccepted = false;
                tradeTarget = null;
                trade = null;
                taking = false;
                tradeAccepted = false;
            }
            else
            {
                if (this != null)
                {
                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Take ended"
                    });
                }
            }
        }
    }
}