#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using db.data;
using wServer.cliPackets;
using wServer.logic;
using wServer.realm.worlds;
using wServer.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    public partial class Player
    {
        private static readonly List<short> Publicbags = new List<short> { 0x0500, 0xffd, 0x0501 };
        private readonly Random invRand = new Random();

        private bool AuditItem(IContainer container, Item item, int slot)
        {
            if (Decision == 1 || Decision == 2 || tradeTarget != null)
                return false;
            return item == null || container.SlotTypes[slot] == 0 || item.SlotType == container.SlotTypes[slot];
        }

        public bool HasSlot(int slot)
        {
            bool ret = false;
            try
            {
                ret = Inventory[3] != null;
            }
            catch
            {
            }
            return ret;
        }

        public bool SwapBackpack(int num)
        {
            if (psr.Character.Backpacks.ContainsKey(num))
            {
                psr.Character.Backpack = num;
                for (int i = 4; i < 12; i++)
                {
                    short itemId = psr.Character.Backpacks[num][i - 4];
                    Inventory[i] = itemId == -1 ? null : XmlDatas.ItemDescs[itemId];
                }
                return true;
            }
            return false;
        }

        /*
         * returns true if packet input is valid
         */
        private bool VerifyInvSwap(InvSwapPacket pkt)
        {
            try
            {
                // get target objects
                Entity en1 = Owner.GetEntity(pkt.Obj1.ObjectId);
                Entity en2 = Owner.GetEntity(pkt.Obj2.ObjectId);

                // check to see if targets are valid
                if ((!(en1 is Player) && !(en1 is Container)) ||
                    (!(en2 is Player) && !(en2 is Container)) || // must be a player or container entity
                    (en1 is Player && en1 != this) ||
                    (en2 is Player && en2 != this)) // if player, must be this player
                {
                    Console.WriteLine("[invSwap:" + nName + "] Targets not valid (" + en1.nName + ", " + en2.nName + ").");
                    return false;
                }
                if (en1 is Container && en2 is Container &&
                    en1 != en2)
                {
                    Console.WriteLine("[invSwap:" + nName + "] Trade between two different containers detected.");
                    return false;
                }

                // check target distance (must be within 1 tile range)
                if (MathsUtils.DistSqr(this.X, this.Y, en1.X, en1.Y) > 1 ||
                    MathsUtils.DistSqr(this.X, this.Y, en2.X, en2.Y) > 1)
                {
                    Console.WriteLine("[invSwap:" + nName + "] Target distance to far from player.");
                    return false;
                }

                // check trade items
                var con1 = en1 as IContainer;
                var con2 = en2 as IContainer;
                Item item1 = con1.Inventory[pkt.Obj1.SlotId]; //TODO: locker
                Item item2 = con2.Inventory[pkt.Obj2.SlotId];
                if (!AuditItem(con2, item1, pkt.Obj2.SlotId) ||
                    !AuditItem(con1, item2, pkt.Obj1.SlotId))
                {
                    Console.WriteLine("[invSwap:" + nName + "] Invalid item swap.");
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[invSwap:" + nName + "] Invalid input.");
                return false;
            }
        }
        public void InventorySwap(RealmTime time, InvSwapPacket pkt)
        {
            // verify packet details
            if (!VerifyInvSwap(pkt))
            {
                this.Client.SendPacket(new InvResultPacket { Result = 1 });
                return;
            }

            // get target objects
            Entity en1 = Owner.GetEntity(pkt.Obj1.ObjectId);
            Entity en2 = Owner.GetEntity(pkt.Obj2.ObjectId);
            var con1 = en1 as IContainer;
            var con2 = en2 as IContainer;

            // get items
            Item item1 = con1.Inventory[pkt.Obj1.SlotId]; //TODO: locker
            Item item2 = con2.Inventory[pkt.Obj2.SlotId];

            // swap items
            con1.Inventory[pkt.Obj1.SlotId] = item2;
            con2.Inventory[pkt.Obj2.SlotId] = item1;

            // check soulbound drop
            if (Publicbags.Contains(en1.ObjectType) && (item2 != null) && (item2.Soulbound || item2.Undead || item2.SUndead))
            {
                DropBag(item2);
                con1.Inventory[pkt.Obj1.SlotId] = null;
            }
            if (Publicbags.Contains(en2.ObjectType) && (item1 != null) && (item1.Soulbound || item1.Undead || item1.SUndead))
            {
                DropBag(item1);
                con2.Inventory[pkt.Obj2.SlotId] = null;
            }

            // update
            en1.UpdateCount++;
            en2.UpdateCount++;

            // update player
            if (en1 is Player || en2 is Player)
            {
                this.CalcBoost();
                this.Client.Save();
                this.Client.SendPacket(new InvResultPacket {Result = 0});
            }

            // log event (needs improvement)
            //Console.WriteLine("[InvSwap:" + nName + "] [" + en1.nName + "," + pkt.Obj1.SlotId + "," + ((item1 == null) ? "" : item1.ObjectId) + "] <-> " +
            //                            "[" + en2.nName + "," + pkt.Obj2.SlotId + "," + ((item2 == null) ? "" : item2.ObjectId) + "]");
        }

        public void InventoryDrop(RealmTime time, InvDropPacket pkt)
        {
            //TODO: locker again
            const short NORM_BAG = 0x0500;
            const short SOUL_BAG = 0x0503;
            const short PDEM_BAG = 0xffd;
            const short DEM_BAG = 0xffe;
            const short SDEM_BAG = 0xfff;

            Entity entity = Owner.GetEntity(pkt.Slot.ObjectId);
            var con = entity as IContainer;
            if (con.Inventory[pkt.Slot.SlotId] == null) return;

            if ((entity is Player) && (entity as Player).Decision == 1)
            {
                (entity as Player).Client.SendPacket(new InvResultPacket {Result = 1});
                return;
            }

            Item item = con.Inventory[pkt.Slot.SlotId];
            con.Inventory[pkt.Slot.SlotId] = null;
            entity.UpdateCount++;

            Container container;
            if (item.Soulbound)
            {
                container = new Container(SOUL_BAG, 1000*60, true) {BagOwner = AccountId};
            }
            else if (item.Undead)
            {
                container = new Container(DEM_BAG, 1000*60, true) {BagOwner = AccountId};
            }
            else if (item.PUndead)
            {
                container = new Container(PDEM_BAG, 1000*60, true);
            }
            else if (item.SUndead)
            {
                container = new Container(SDEM_BAG, 1000*60, true) {BagOwner = AccountId};
            }
            else
            {
                container = new Container(NORM_BAG, 1000*60, true);
            }
            float bagx = entity.X + (float) ((invRand.NextDouble()*2 - 1)*0.5);
            float bagy = entity.Y + (float) ((invRand.NextDouble()*2 - 1)*0.5);
            try
            {
                container.Inventory[0] = item;
                container.Move(bagx, bagy);
                container.Size = 75;
                Owner.EnterWorld(container);

                if (entity is Player)
                {
                    (entity as Player).CalcBoost();
                    (entity as Player).Client.SendPacket(new InvResultPacket
                    {
                        Result = 0
                    });
                    (entity as Player).Client.Save();
                }

                if (Owner is Vault)
                    if ((Owner as Vault).psr.Account.Name == psr.Account.Name)
                        return;

                const string dir = @"logs";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                using (var writer = new StreamWriter(@"logs\DropLog.log", true))
                {
                    writer.WriteLine(Name + " dropped a " + item.ObjectId +
                                     (container.BagOwner != null ? " (Soulbound)" : ""));
                }
            }
            catch
            {
                Console.Out.WriteLine(Name + " just attempted to dupe.");
            }
        }

        public void DropBag(Item i)
        {
            short bagId = 0x0500;
            bool soulbound = false;
            if (i.Soulbound)
            {
                bagId = 0x0503;
                soulbound = true;
            }
            else if (i.Undead)
            {
                bagId = 0xffe;
                soulbound = true;
            }
            else if (i.SUndead)
            {
                bagId = 0xfff;
                soulbound = true;
            }
            else if (i.PUndead)
            {
                bagId = 0xffd;
            }

            var container = new Container(bagId, 1000*60, true);
            if (soulbound)
                container.BagOwner = AccountId;
            container.Inventory[0] = i;
            container.Move(X + (float) ((invRand.NextDouble()*2 - 1)*0.5),
                Y + (float) ((invRand.NextDouble()*2 - 1)*0.5));
            container.Size = 75;
            Owner.EnterWorld(container);

            if (Owner is Vault)
                if ((Owner as Vault).psr.Account.Name == psr.Account.Name)
                    return;

            const string dir = @"logs";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var writer = new StreamWriter(@"logs\DropLog.log", true))
            {
                writer.WriteLine(Name + " dropped a " + i.ObjectId + (soulbound ? " (Soulbound)" : ""));
            }
        }
    }
}