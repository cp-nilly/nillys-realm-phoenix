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
        public bool forging;
        public List<short> forgableItems = new List<short>();
        public List<string> requestedItems = new List<string>();
        public List<string> playerItems = new List<string>();
        public void RequestForge(RealmTime time, RequestTradePacket pkt)
        {
            if (tradeTarget != null)
            {
                SendError("You're already trading!");
                tradeTarget = null;
                return;
            }
            trade = new bool[12];
            tradeAccepted = false;
            forging = true;
            ForgeList f = new ForgeList();
            foreach (var i in f.combos)
            {
                for (var x = 0; x < i.Key.Length; x++)
                {
                    requestedItems.Add(i.Key[x]);
                }
                for (var x = 0; x < Inventory.Length; x++)
                {
                    if (Inventory[x] != null)
                    {
                        playerItems.Add(Inventory[x].ObjectId);
                    }
                }
                for (var x = 0; x < playerItems.Count; x++)
                {
                    for (var y = 0; y < requestedItems.Count; y++)
                    {
                        if (requestedItems[y] == playerItems[x])
                        {
                            requestedItems.RemoveAt(y);
                            break;
                        }
                    }
                }
                if (requestedItems.Count == 0)
                {
                    var icdatas = new Dictionary<string, short>(db.data.XmlDatas.IdToType, StringComparer.OrdinalIgnoreCase);
                    short objType;
                    icdatas.TryGetValue(i.Value, out objType);
                    forgableItems.Add(objType);
                }
            }
            if (forgableItems.Count < Inventory.Length)
            {
                for (var i = forgableItems.Count; i < Inventory.Length; i++)
                {
                    forgableItems.Add(-1);
                }
            }
            var my = new TradeItem[Inventory.Length];
            for (var i = 0; i < Inventory.Length; i++)
            {
                my[i] = new TradeItem
                {
                    Item = forgableItems[i],
                    SlotType = SlotTypes[0],
                    Included = false,
                    Tradeable = forgableItems[i] != -1
                };
            }
            var your = new TradeItem[12];
            for (var i = 0; i < Inventory.Length; i++)
                your[i] = new TradeItem
                {
                    Item = Inventory[i] == null ? -1 : Inventory[i].ObjectType,
                    SlotType = SlotTypes[i],
                    Included = false,
                    Tradeable = false
                };
            psr.SendPacket(new TradeStartPacket
            {
                MyItems = my,
                YourName = "Items",
                YourItems = your
            });
        }

        private void ForgeTick(RealmTime time)
        {
            if (trade != null)
                if (forging == true)
                        if (tradeAccepted)
                            {
                                name1 = Name;
                                DoForge();
                            }

        }
        private void DoForge()
        {
            if (Owner != null)
            {
                var thisItems = new List<Item>();
                var targetItems = new List<Item>();
                for (var i = 0; i < trade.Length; i++)
                    if (trade[i])
                    {
                        if (Inventory[i] != null)
                        {
                            thisItems.Add(Inventory[i]);
                            Inventory[i] = null;
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
                for (var i = 0; i < Inventory.Length; i++)
                {
                    targetItems.Add(Inventory[i]);
                    Inventory[i] = null;

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
                if (targetItems.Count == 0)
                    targetItems.Add(null);
                if (thisItems.Count == 0)
                    thisItems.Add(null);
                
                for (var i = 0; i < Inventory.Length; i++) //put items by slotType
                    if (Inventory[i] == null)
                    {
                        if (SlotTypes[i] == 0 && thisItems[0] != null)
                        {
                            Inventory[i] = thisItems[0];
                            thisItems.RemoveAt(0);
                        }
                        else
                        {
                            var itmIdx = -1;
                            for (var j = 0; j < thisItems.Count; j++)
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
                    for (var i = 0; i < Inventory.Length; i++) //force put item
                        if (Inventory[i] == null)
                        {
                            Inventory[i] = thisItems[0];
                            thisItems.RemoveAt(0);
                            if (thisItems.Count == 0) break;
                        }
                psr.SendPacket(new TradeDonePacket
                {
                    Result = 1,
                    Message = "Items Forged"
                });

                const string dir = @"logs";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                using (var writer = new StreamWriter(@"logs\ForgeLog.log", true))
                {
                    writer.WriteLine(Name + " forged " + "{" + items1 + "}" + " together to make " + "{" + items2 + "}");
                }
                Console.Out.WriteLine(Name + " forged " + "{" + items1 + "}" + " together to make " + "{" + items2 + "}");
                items1 = "";
                items2 = "";
                itemnumber1 = 0;
                itemnumber2 = 0;
                UpdateCount++;
                name1 = "";
                name2 = "";
                trade = null;
                forging = false;
                tradeAccepted = false;
                playerItems.Clear();
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