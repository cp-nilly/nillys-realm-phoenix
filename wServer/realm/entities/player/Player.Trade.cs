#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using wServer.cliPackets;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        private readonly bool[] inUse = new bool[12];
        private readonly List<string> itemsNeeded = new List<string>();
        private readonly Dictionary<Player, int> potentialTrader = new Dictionary<Player, int>();
        private ForgeList f = new ForgeList();
        private int itemnumber1;
        private int itemnumber2;
        private bool[] itemresult;
        private string name1;
        private string name2;
        public bool taking;
        private bool[] trade;
        private bool tradeAccepted;
        public Player tradeTarget;
        public static string items1 { get; set; }
        public static string items2 { get; set; }

        public void RequestTrade(RealmTime time, RequestTradePacket pkt)
        {
            if (!NameChosen)
            {
                SendInfo("Unique name is required to trade with others!");
                return;
            }
            Player target = Owner.GetUniqueNamedPlayer(pkt.Name);
            if (intake)
            {
                SendError(target.Name + " is already trading!");
                return;
            }
            if (tradeTarget != null)
            {
                SendError("You're already trading!");
                tradeTarget = null;
                return;
            }
            //if (psr.Player == target)
            //{
            //    SendError("Trading with yourself would be pointless.");
            //    tradeTarget = null;
            //    return;
            //}
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

            if (potentialTrader.ContainsKey(target))
            {
                tradeTarget = target;
                trade = new bool[12];
                tradeAccepted = false;
                target.tradeTarget = this;
                target.trade = new bool[12];
                target.tradeAccepted = false;
                potentialTrader.Clear();
                target.potentialTrader.Clear();
                taking = false;

                var my = new TradeItem[Inventory.Length];
                for (int i = 0; i < Inventory.Length; i++)
                    my[i] = new TradeItem
                    {
                        Item = Inventory[i] == null ? -1 : Inventory[i].ObjectType,
                        SlotType = SlotTypes[i],
                        Included = false,
                        Tradeable =
                            (Inventory[i] != null && i >= 4) &&
                            (!Inventory[i].Soulbound && !Inventory[i].Undead && !Inventory[i].SUndead)
                    };
                var your = new TradeItem[target.Inventory.Length];
                for (int i = 0; i < target.Inventory.Length; i++)
                    your[i] = new TradeItem
                    {
                        Item = target.Inventory[i] == null ? -1 : target.Inventory[i].ObjectType,
                        SlotType = target.SlotTypes[i],
                        Included = false,
                        Tradeable =
                            (target.Inventory[i] != null && i >= 4) &&
                            (!target.Inventory[i].Soulbound && !target.Inventory[i].Undead &&
                             !target.Inventory[i].SUndead)
                    };

                psr.SendPacket(new TradeStartPacket
                {
                    MyItems = my,
                    YourName = target.Name,
                    YourItems = your
                });
                target.psr.SendPacket(new TradeStartPacket
                {
                    MyItems = your,
                    YourName = Name,
                    YourItems = my
                });
            }
            else
            {
                target.potentialTrader[this] = 1000*20;
                target.psr.SendPacket(new TradeRequestedPacket
                {
                    Name = Name
                });
                SendInfo("Sent trade request to " + target.Name);
            }
        }

        public bool usable(int slot, string[] needed)
        {
            if (inUse[slot])
            {
                return false;
            }
            if (Inventory[slot] == null)
            {
                return false;
            }
            for (int i = 0; i < needed.Length; i++)
            {
                if (Inventory[slot].ObjectId != needed[i])
                {
                    if (i == needed.Length)
                    {
                        return false;
                    }
                }
                else
                {
                    inUse[slot] = true;
                    itemsNeeded.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void ChangeTrade(RealmTime time, ChangeTradePacket pkt)
        {
            if (trade != pkt.Offers)
            {
                tradeAccepted = false;
                tradeTarget.tradeAccepted = false;
                trade = pkt.Offers;

                tradeTarget.psr.SendPacket(new TradeChangedPacket
                {
                    Offers = trade
                });
            }
        }

        public void AcceptTrade(RealmTime time, AcceptTradePacket pkt)
        {
            trade = pkt.MyOffers;
            if (tradeTarget.trade.SequenceEqual(pkt.YourOffers))
            {
                tradeTarget.trade = pkt.YourOffers;
                tradeAccepted = true;
                tradeTarget.psr.SendPacket(new TradeAcceptedPacket
                {
                    MyOffers = tradeTarget.trade,
                    YourOffers = trade
                });
                Console.Out.WriteLine("Player {0} accepted trade with {1}", nName, tradeTarget.nName);

                /*if (this.tradeAccepted && tradeTarget.tradeAccepted)
                {
                    DoTrade();
                    Console.Out.WriteLine("Did trade!");
                }*/
            }
        }

        public void CancelTrade(RealmTime time, CancelTradePacket pkt)
        {
            {
                psr.SendPacket(new TradeDonePacket
                {
                    Result = 1,
                    Message = "Trade cancelled."
                });
                tradeTarget.psr.SendPacket(new TradeDonePacket
                {
                    Result = 1,
                    Message = "Trade cancelled."
                });
                tradeTarget.tradeTarget = null;
                tradeTarget.trade = null;
                tradeTarget.tradeAccepted = false;
                tradeTarget = null;
                trade = null;
                tradeAccepted = false;
            }
        }

        private void TradeTick(RealmTime time)
        {
            if (trade != null)
                if (taking == false)
                    if (tradeTarget != null)
                        if (tradeAccepted && tradeTarget.tradeAccepted)
                            if (tradeTarget != null && Owner != null && tradeTarget.Owner != null &&
                                Owner == tradeTarget.Owner)
                            {
                                name1 = Name;
                                name2 = tradeTarget.Name;
                                DoTrade();
                            }
                            else
                            {
                                tradeTarget.tradeTarget = null;
                                tradeTarget.trade = null;
                                tradeTarget.tradeAccepted = false;
                                tradeTarget = null;
                                trade = null;
                                tradeAccepted = false;
                                return;
                            }
            CheckTradeTimeout(time);
        }

        private void CheckTradeTimeout(RealmTime time)
        {
            List<Tuple<Player, int>> newState =
                potentialTrader.Select(i => new Tuple<Player, int>(i.Key, i.Value - time.thisTickTimes)).ToList();

            foreach (var i in newState)
            {
                if (i.Item2 < 0)
                {
                    {
                        i.Item1.SendError("Trade to " + Name + " has timed out!");
                    }
                    potentialTrader.Remove(i.Item1);
                }
                else potentialTrader[i.Item1] = i.Item2;
            }
        }

        private void DoTrade()
        {
            if (tradeTarget != null && Owner != null && tradeTarget.Owner != null && Owner == tradeTarget.Owner)
            {
                int thisemptyslots = 0;
                int targetemptyslots = 0;
                var thisItems = new List<Item>();
                for (int i = 0; i < trade.Length; i++)
                    if (trade[i])
                    {
                        thisItems.Add(Inventory[i]);
                        Inventory[i] = null;
                        UpdateCount++;
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

                var targetItems = new List<Item>();
                for (int i = 0; i < tradeTarget.trade.Length; i++)
                    if (tradeTarget.trade[i])
                    {
                        targetItems.Add(tradeTarget.Inventory[i]);
                        tradeTarget.Inventory[i] = null;
                        tradeTarget.UpdateCount++;

                        if (itemnumber2 == 0)
                        {
                            items2 = items2 + " " + targetItems[itemnumber2].ObjectId;
                        }
                        else if (itemnumber2 > 0)
                        {
                            items2 = items2 + ", " + targetItems[itemnumber2].ObjectId;
                        }
                        itemnumber2++;
                    }


                for (int i = 0; i != Inventory.Length; i++)
                {
                    if (Inventory[i] == null)
                    {
                        if (SlotTypes[i] == 0)
                        {
                            thisemptyslots++;
                        }
                        else
                        {
                            for (int j = 0; j < targetItems.Count; j++)
                            {
                                if (targetItems[j].SlotType == SlotTypes[i])
                                {
                                    thisemptyslots++;
                                    break;
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i != tradeTarget.Inventory.Length; i++)
                {
                    if (SlotTypes[i] == 0)
                    {
                        targetemptyslots++;
                    }
                    else
                    {
                        for (int j = 0; j < thisItems.Count; j++)
                        {
                            try
                            {
                                if (thisItems[j].SlotType == SlotTypes[i])
                                {
                                    targetemptyslots++;
                                    break;
                                }
                            }
                            catch (Exception)
                            {
                                psr.SendPacket(new TradeDonePacket
                                 {
                                     Result = 1,
                                     Message = "Trade unsuccessful."
                                 });
                            }
                        }
                    }
                }
                if (targetemptyslots >= thisItems.Count && thisemptyslots >= targetItems.Count)
                {
                    if (targetItems.Count == 0)
                        targetItems.Add(null);
                    if (thisItems.Count == 0)
                        thisItems.Add(null);
                    for (int i = 0; i < Inventory.Length; i++) //put items by slotType
                        if (Inventory[i] == null)
                        {
                            if (SlotTypes[i] == 0)
                            {
                                Inventory[i] = targetItems[0];
                                targetItems.RemoveAt(0);
                            }
                            else
                            {
                                int itmIdx = -1;
                                for (int j = 0; j < targetItems.Count; j++)
                                {
                                    try
                                    {
                                        if (targetItems[j].SlotType == SlotTypes[i])
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
                                    Inventory[i] = targetItems[itmIdx];
                                    targetItems.RemoveAt(itmIdx);
                                }
                            }
                            if (targetItems.Count == 0) break;
                        }
                    if (targetItems.Count > 0)
                        for (int i = 0; i < Inventory.Length; i++) //force put item
                            if (Inventory[i] == null)
                            {
                                Inventory[i] = targetItems[0];
                                targetItems.RemoveAt(0);
                                if (targetItems.Count == 0) break;
                            }


                    for (int i = 0; i < tradeTarget.Inventory.Length; i++) //put items by slotType
                        if (tradeTarget.Inventory[i] == null)
                        {
                            if (tradeTarget.SlotTypes[i] == 0)
                            {
                                tradeTarget.Inventory[i] = thisItems[0];
                                thisItems.RemoveAt(0);
                            }
                            else
                            {
                                int itmIdx = -1;
                                for (int j = 0; j < thisItems.Count; j++)
                                {
                                    try
                                    {
                                        if (thisItems[j].SlotType == tradeTarget.SlotTypes[i])
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
                                    tradeTarget.Inventory[i] = thisItems[itmIdx];
                                    thisItems.RemoveAt(itmIdx);
                                }
                            }
                            if (thisItems.Count == 0) break;
                        }
                    if (thisItems.Count > 0)
                        for (int i = 0; i < tradeTarget.Inventory.Length; i++) //force put item
                            if (tradeTarget.Inventory[i] == null)
                            {
                                tradeTarget.Inventory[i] = thisItems[0];
                                thisItems.RemoveAt(0);
                                if (thisItems.Count == 0) break;
                            }


                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Trade successful!"
                    });
                    tradeTarget.psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Trade successful!"
                    });
                    SaveToCharacter();
                    psr.Save();
                    tradeTarget.SaveToCharacter();
                    tradeTarget.psr.Save();

                    const string dir = @"logs";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    using (var writer = new StreamWriter(@"logs\TradeLog.log", true))
                    {
                        writer.WriteLine(Name + " traded " + "{" + items1 + "}" + " with " + tradeTarget.Name + " for " +
                                         "{" + items2 + "}");
                    }
                    Console.Out.WriteLine(Name + " traded " + "{" + items1 + "}" + " with " + tradeTarget.Name + " for " +
                                          "{" + items2 + "}");
                    items1 = "";
                    items2 = "";
                    itemnumber1 = 0;
                    itemnumber2 = 0;
                    UpdateCount++;
                    tradeTarget.UpdateCount++;
                    name1 = "";
                    name2 = "";
                    tradeTarget.tradeTarget = null;
                    tradeTarget.trade = null;
                    tradeTarget.tradeAccepted = false;
                    tradeTarget = null;
                    trade = null;
                    tradeAccepted = false;
                }
                else
                {
                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Exploit Halted! You have been logged."
                    });
                    tradeTarget.psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Exploit Halted! You have been logged."
                    });
                    const string dir = @"logs";
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    using (var writer = new StreamWriter(@"logs\ExploitLog.log", true))
                    {
                        writer.WriteLine(Name + " tried to trade an ability into a weapon slot with " + tradeTarget.Name +
                                         " (Error: More items than free inventory slots)");
                    }
                    Console.Out.WriteLine(Name + " tried to trade an ability into a weapon slot with " +
                                          tradeTarget.Name + " (Error: More items than free inventory slots)");
                    items1 = "";
                    items2 = "";
                    itemnumber1 = 0;
                    itemnumber2 = 0;
                    UpdateCount++;
                    tradeTarget.UpdateCount++;
                    name1 = "";
                    name2 = "";
                    tradeTarget.tradeTarget = null;
                    tradeTarget.trade = null;
                    tradeTarget.tradeAccepted = false;
                    tradeTarget = null;
                    trade = null;
                    tradeAccepted = false;
                }
            }
            else
            {
                if (this != null)
                {
                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Exploit Halted! You have been logged."
                    });
                }
                if (tradeTarget != null)
                {
                    tradeTarget.psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = "Exploit Halted! You have been logged."
                    });
                }
                const string dir = @"logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                using (var writer = new StreamWriter(@"logs\ExploitLog.log", true))
                {
                    writer.WriteLine(name1 + " tried to trade an ability into a weapon slot with " + name2 +
                                     " (Error: More items than free inventory slots)");
                }
                Console.Out.WriteLine(name1 + " tried to trade an ability into a weapon slot with " + name2 +
                                      " (Error: More items than free inventory slots)");
                items1 = "";
                items2 = "";
                name1 = "";
                name2 = "";
                itemnumber1 = 0;
                itemnumber2 = 0;
                UpdateCount++;
                tradeTarget.UpdateCount++;
                tradeTarget.tradeTarget = null;
                tradeTarget.trade = null;
                tradeTarget.tradeAccepted = false;
                tradeTarget = null;
                trade = null;
                tradeAccepted = false;
            }
        }
    }
}