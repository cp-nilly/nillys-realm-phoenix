#region

using System;
using System.Collections.Generic;
using db.data;
using wServer.logic.loot;
using wServer.realm.entities;

#endregion

namespace wServer.realm.setpieces
{
    internal class Oasis : ISetPiece
    {
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Light Grass"];
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];
        private static readonly short Tree = XmlDatas.IdToType["Palm Tree"];

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
            int outerRadius = 13;
            int waterRadius = 10;
            int islandRadius = 3;
            var border = new List<IntPoint>();

            var t = new int[Size, Size];
            for (int y = 0; y < Size; y++) //Outer
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= outerRadius)
                        t[x, y] = 1;
                }

            for (int y = 0; y < Size; y++) //Water
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= waterRadius)
                    {
                        t[x, y] = 2;
                        if (waterRadius - r < 1)
                            border.Add(new IntPoint(x, y));
                    }
                }

            for (int y = 0; y < Size; y++) //Island
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= islandRadius)
                    {
                        t[x, y] = 1;
                        if (islandRadius - r < 1)
                            border.Add(new IntPoint(x, y));
                    }
                }

            var trees = new HashSet<IntPoint>();
            while (trees.Count < border.Count*0.5)
                trees.Add(border[rand.Next(0, border.Count)]);

            foreach (IntPoint i in trees)
                t[i.X, i.Y] = 3;

            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = Tree;
                        tile.Name = "size:" + (rand.Next()%2 == 0 ? 120 : 140);
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }

            Entity giant = Entity.Resolve(0x678);
            giant.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(giant);

            var container = new Container(0x0501, null, false);
            int count = rand.Next(5, 8);
            var items = new List<Item>();
            while (items.Count < count)
            {
                Item item = chest.GetRandomLoot(rand);
                if (item != null) items.Add(item);
            }
            for (int i = 0; i < items.Count; i++)
                container.Inventory[i] = items[i];
            container.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(container);
        }
    }
}