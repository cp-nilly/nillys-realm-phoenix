#region

using System;
using System.Collections.Generic;
using db.data;
using wServer.logic.loot;
using wServer.realm.entities;

#endregion

namespace wServer.realm.setpieces
{
    internal class Graveyard : ISetPiece
    {
        protected static readonly byte Floor = (byte) XmlDatas.IdToType["Grass"];
        protected static readonly short WallA = XmlDatas.IdToType["Grey Wall"];
        protected static readonly short WallB = XmlDatas.IdToType["Destructible Grey Wall"];
        protected static readonly short Cross = XmlDatas.IdToType["Cross"];

        protected static readonly LootDef chest = new LootDef(0, 0, 0, 0,
            Tuple.Create(0.1, (ILoot) new TierLoot(4, ItemType.Weapon)),
            Tuple.Create(0.05, (ILoot) new TierLoot(5, ItemType.Weapon)),
            Tuple.Create(0.01, (ILoot) new TierLoot(6, ItemType.Weapon)),
            Tuple.Create(0.1, (ILoot) new TierLoot(3, ItemType.Armor)),
            Tuple.Create(0.05, (ILoot) new TierLoot(4, ItemType.Armor)),
            Tuple.Create(0.01, (ILoot) new TierLoot(5, ItemType.Armor)),
            Tuple.Create(0.1, (ILoot) new TierLoot(1, ItemType.Ability)),
            Tuple.Create(0.05, (ILoot) new TierLoot(2, ItemType.Ability)),
            Tuple.Create(0.01, (ILoot) new TierLoot(3, ItemType.Ability)),
            Tuple.Create(0.1, (ILoot) new TierLoot(2, ItemType.Ring)),
            Tuple.Create(0.01, (ILoot) new TierLoot(3, ItemType.Ring)),
            Tuple.Create(0.05, (ILoot) HpPotionLoot.Instance),
            Tuple.Create(0.05, (ILoot) MpPotionLoot.Instance)
            );

        private readonly Random rand = new Random();

        public int Size
        {
            get { return 34; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[23, 35];

            for (var x = 0; x < 23; x++) //Floor
                for (var y = 0; y < 35; y++)
                    t[x, y] = rand.Next()%3 == 0 ? 0 : 1;

            for (var y = 0; y < 35; y++) //Perimeters
                t[0, y] = t[22, y] = 2;
            for (var x = 0; x < 23; x++)
                t[x, 0] = t[x, 34] = 2;

            var pts = new List<IntPoint>();
            for (var y = 0; y < 11; y++) //Crosses
                for (var x = 0; x < 7; x++)
                {
                    if (rand.Next()%3 > 0)
                        t[2 + 3*x, 2 + 3*y] = 4;
                    else
                        pts.Add(new IntPoint(2 + 3*x, 2 + 3*y));
                }

            for (var x = 0; x < 23; x++) //Corruption
                for (var y = 0; y < 35; y++)
                {
                    if (t[x, y] == 1 || t[x, y] == 0 || t[x, y] == 4) continue;
                    var p = rand.NextDouble();
                    if (p < 0.1)
                        t[x, y] = 1;
                    else if (p < 0.4)
                        t[x, y]++;
                }


            //Boss & Chest
            var pt = pts[rand.Next(0, pts.Count)];
            t[pt.X, pt.Y] = 5;
            t[pt.X + 1, pt.Y] = 6;

            var r = rand.Next(0, 4);
            for (var i = 0; i < r; i++) //Rotation
                t = SetPieces.rotateCW(t);
            int w = t.GetLength(0), h = t.GetLength(1);

            for (var x = 0; x < w; x++) //Rendering
                for (var y = 0; y < h; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = WallA;
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                        var wall = Entity.Resolve(WallB);
                        wall.Move(x + pos.X + 0.5f, y + pos.Y + 0.5f);
                        world.EnterWorld(wall);
                    }
                    else if (t[x, y] == 4)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = Cross;
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 5)
                    {
                        var container = new Container(0x0501, null, false);
                        var count = rand.Next(3, 8);
                        var items = new List<Item>();
                        while (items.Count < count)
                        {
                            var item = chest.GetRandomLoot(rand);
                            if (item != null) items.Add(item);
                        }
                        for (var i = 0; i < items.Count; i++)
                            container.Inventory[i] = items[i];
                        container.Move(pos.X + x + 0.5f, pos.Y + y + 0.5f);
                        world.EnterWorld(container);
                    }
                    else if (t[x, y] == 6)
                    {
                        var mage = Entity.Resolve(0x0925);
                        mage.Move(pos.X + x, pos.Y + y);
                        world.EnterWorld(mage);
                    }
                }


            //Boss & Chest
        }
    }
}