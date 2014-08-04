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
            Console.WriteLine("[cancelTrade:" + nName + "] " + pkt);
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
            string failedMsg = "Error while trading. Trade unsuccessful.";
            string msg = "Trade Successful!";
            var thisItems = new List<Item>();
            var targetItems = new List<Item>();

            // make sure trade targets are valid
            if (tradeTarget == null || Owner == null || tradeTarget.Owner == null || Owner != tradeTarget.Owner)
            {
                if (this != null)
                    psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = failedMsg
                    });

                if (tradeTarget != null)
                    tradeTarget.psr.SendPacket(new TradeDonePacket
                    {
                        Result = 1,
                        Message = failedMsg
                    });

                //TODO - logThis
                return;
            }

            // get trade items
            for (int i = 4; i < Inventory.Length; i++)
            {
                if (trade[i] && !Inventory[i].Soulbound)
                {
                    thisItems.Add(Inventory[i]);
                    Inventory[i] = null;
                    UpdateCount++;

                    // save this trade info
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


                if (tradeTarget.trade[i] && !tradeTarget.Inventory[i].Soulbound)
                {
                    targetItems.Add(tradeTarget.Inventory[i]);
                    tradeTarget.Inventory[i] = null;
                    tradeTarget.UpdateCount++;

                    // save target trade info
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
            }

            // move thisItems -> tradeTarget
            for (int j = thisItems.Count - 1; j >= 0; j--)
                for (int i = 0; i < tradeTarget.Inventory.Length; i++)
                {
                    if ((tradeTarget.SlotTypes[i] == 0 &&
                            tradeTarget.Inventory[i] == null) ||
                        (thisItems[j] != null &&
                            tradeTarget.SlotTypes[i] == thisItems[j].SlotType &&
                            tradeTarget.Inventory[i] == null))
                    {
                        tradeTarget.Inventory[i] = thisItems[j];
                        thisItems.RemoveAt(j);
                        break;
                    }
                }

            // move tradeItems -> this
            for (int j = targetItems.Count - 1; j >= 0; j--)
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if ((SlotTypes[i] == 0 &&
                            Inventory[i] == null) ||
                        (targetItems[j] != null &&
                            SlotTypes[i] == targetItems[j].SlotType &&
                            Inventory[i] == null))
                    {
                        Inventory[i] = targetItems[j];
                        targetItems.RemoveAt(j);
                        break;
                    }
                }

            // check for lingering items
            if (thisItems.Count > 0 ||
                targetItems.Count > 0)
            {
                msg = "An error occured while trading! Some items were lost!";
            }

            // trade successful, notify and save
            psr.SendPacket(new TradeDonePacket
            {
                Result = 1,
                Message = msg
            });
            tradeTarget.psr.SendPacket(new TradeDonePacket
            {
                Result = 1,
                Message = msg
            });
            SaveToCharacter();
            psr.Save();
            tradeTarget.SaveToCharacter();
            tradeTarget.psr.Save();

            // log trade
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

            // clean up
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
}