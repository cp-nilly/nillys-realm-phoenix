#region

using System;
using System.Collections.Generic;
using db.data;
using wServer.logic.loot;
using wServer.realm.entities;

#endregion

namespace wServer.realm.setpieces
{
    internal class Pyre : ISetPiece
    {
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Scorch Blend"];

        private static readonly LootDef chest = new LootDef(0, 0, 0, 0,
            Tuple.Create(0.2, (ILoot) new TierLoot(5, ItemType.Weapon)),
            Tuple.Create(0.1, (ILoot) new TierLoot(6, ItemType.Weapon)),
            Tuple.Create(0.01, (ILoot) new TierLoot(7, ItemType.Weapon)),
            Tuple.Create(0.2, (ILoot) new TierLoot(4, ItemType.Armor)),
            Tuple.Create(0.1, (ILoot) new TierLoot(5, ItemType.Armor)),
            Tuple.Create(0.05, (ILoot) new TierLoot(6, ItemType.Armor)),
            Tuple.Create(0.2, (ILoot) new TierLoot(2, ItemType.Ability)),
            Tuple.Create(0.01, (ILoot) new TierLoot(3, ItemType.Ability)),
            Tuple.Create(0.15, (ILoot) new TierLoot(1, ItemType.Ring)),
            Tuple.Create(0.05, (ILoot) new TierLoot(2, ItemType.Ring)),
            Tuple.Create(0.1, (ILoot) HpPotionLoot.Instance),
            Tuple.Create(0.1, (ILoot) MpPotionLoot.Instance)
            );

        private readonly Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            for (var x = 0; x < Size; x++)
                for (var y = 0; y < Size; y++)
                {
                    var dx = x - (Size/2.0);
                    var dy = y - (Size/2.0);
                    var r = Math.Sqrt(dx*dx + dy*dy) + rand.NextDouble()*4 - 2;
                    if (r <= 10)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }

            var lord = Entity.Resolve(0x675);
            lord.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(lord);

            var container = new Container(0x0501, null, false);
            var count = rand.Next(5, 8);
            var items = new List<Item>();
            while (items.Count < count)
            {
                var item = chest.GetRandomLoot(rand);
                if (item != null) items.Add(item);
            }
            for (var i = 0; i < items.Count; i++)
                container.Inventory[i] = items[i];
            container.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(container);
        }
    }
}