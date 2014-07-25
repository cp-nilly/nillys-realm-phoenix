#region

using System;
using System.Collections.Generic;
using db.data;
using wServer.logic.loot;
using wServer.realm.entities;

#endregion

namespace wServer.realm.setpieces
{
    internal class LavaFissure : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly short Floor = XmlDatas.IdToType["Partial Red Floor"];

        private static readonly LootDef chest = new LootDef(0, 0, 0, 0,
            Tuple.Create(0.2, (ILoot) new TierLoot(7, ItemType.Weapon)),
            Tuple.Create(0.1, (ILoot) new TierLoot(8, ItemType.Weapon)),
            Tuple.Create(0.01, (ILoot) new TierLoot(9, ItemType.Weapon)),
            Tuple.Create(0.2, (ILoot) new TierLoot(6, ItemType.Armor)),
            Tuple.Create(0.1, (ILoot) new TierLoot(7, ItemType.Armor)),
            Tuple.Create(0.05, (ILoot) new TierLoot(8, ItemType.Armor)),
            Tuple.Create(0.2, (ILoot) new TierLoot(2, ItemType.Ability)),
            Tuple.Create(0.1, (ILoot) new TierLoot(3, ItemType.Ability)),
            Tuple.Create(0.01, (ILoot) new TierLoot(4, ItemType.Ability)),
            Tuple.Create(0.1, (ILoot) new TierLoot(2, ItemType.Ring)),
            Tuple.Create(0.05, (ILoot) new TierLoot(3, ItemType.Ring)),
            Tuple.Create(0.1, (ILoot) HpPotionLoot.Instance),
            Tuple.Create(0.1, (ILoot) MpPotionLoot.Instance)
            );

        private readonly Random rand = new Random();

        public int Size
        {
            get { return 40; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var p = new int[Size, Size];
            const double SCALE = 5.5;
            for (int x = 0; x < Size; x++) //Lava
            {
                double t = (double) x/Size*Math.PI;
                double x_ = t/Math.Sqrt(2) - Math.Sin(t)/(SCALE*Math.Sqrt(2));
                double y1 = t/Math.Sqrt(2) - 2*Math.Sin(t)/(SCALE*Math.Sqrt(2));
                double y2 = t/Math.Sqrt(2) + Math.Sin(t)/(SCALE*Math.Sqrt(2));
                y1 /= Math.PI/Math.Sqrt(2);
                y2 /= Math.PI/Math.Sqrt(2);

                var y1_ = (int) Math.Ceiling(y1*Size);
                var y2_ = (int) Math.Floor(y2*Size);
                for (int i = y1_; i < y2_; i++)
                    p[x, i] = 1;
            }

            for (int x = 0; x < Size; x++) //Floor
                for (int y = 0; y < Size; y++)
                {
                    if (p[x, y] == 1 && rand.Next()%5 == 0)
                        p[x, y] = 2;
                }

            int r = rand.Next(0, 4); //Rotation
            for (int i = 0; i < r; i++)
                p = SetPieces.rotateCW(p);
            p[20, 20] = 2;

            for (int x = 0; x < Size; x++) //Rendering
                for (int y = 0; y < Size; y++)
                {
                    if (p[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (p[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = Floor;
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }


            Entity demon = Entity.Resolve(0x668);
            demon.Move(pos.X + 20.5f, pos.Y + 20.5f);
            world.EnterWorld(demon);

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
            container.Move(pos.X + 20.5f, pos.Y + 20.5f);
            world.EnterWorld(container);
        }
    }
}