#region

using System;
using System.Collections.Generic;
using db.data;

#endregion

namespace wServer.realm.setpieces
{
    internal class Hermit : ISetPiece
    {
        protected static readonly byte DarkGrass = (byte) XmlDatas.IdToType["Dark Grass"];
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];
        private static readonly byte Pillar = (byte) XmlDatas.IdToType["Grey Pillar"];
        private static readonly byte BrokenPillar = (byte) XmlDatas.IdToType["Broken Grey Pillar"];
        private static readonly byte WaterDeep = (byte) XmlDatas.IdToType["Dark Water"];
        protected static readonly short Flower = XmlDatas.IdToType["Jungle Ground Flowers"];
        private static readonly byte Sand = (byte) XmlDatas.IdToType["Dark Sand"];


        private readonly Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            int DarkGrassradiu = 14;
            int sandRadius = 12;
            int waterRadius = 9;
            float deepWaterRadius = 4f;

            var border = new List<IntPoint>();

            const int bas = 17;
            var o = new int[Size, Size];
            var t = new int[Size, Size];


            for (int x = 0; x < Size; x++) //Flooring
                for (int y = 0; y < Size; y++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy) + rand.NextDouble()*4 - 2;
                    if (r <= DarkGrassradiu)
                        t[x, y] = 1;
                }

            for (int y = 0; y < Size; y++) //Outer
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= sandRadius)
                        t[x, y] = 2;
                }
            for (int y = 0; y < Size; y++) //Water
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= waterRadius)
                    {
                        t[x, y] = 3;
                    }
                }


            for (int y = 0; y < Size; y++) //Deep Water
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= deepWaterRadius)
                    {
                        t[x, y] = 4;
                    }
                }
            for (int x = 0; x < Size; x++) //Plants
                for (int y = 0; y < Size; y++)
                {
                    if (((x > 5 && x < bas) || (x < Size - 5 && x > Size - bas) ||
                         (y > 5 && y < bas) || (y < Size - 5 && y > Size - bas)) &&
                        o[x, y] == 0 && t[x, y] == 1)
                    {
                        double r = rand.NextDouble();
                        if (r > 0.6) //0.4
                            o[x, y] = 4;
                        else if (r > 0.35) //0.25
                            o[x, y] = 5;
                        else if (r > 0.33) //0.02
                            o[x, y] = 6;
                    }
                }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = DarkGrass;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Sand;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 4)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = WaterDeep;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (o[x, y] == 5)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.ObjType = Flower;
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 6)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = BrokenPillar;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 7)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Pillar;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
            Entity hermit = Entity.Resolve(0x0d61);

            Entity tentacle1 = Entity.Resolve(0x0d65);

            Entity tentacle2 = Entity.Resolve(0x0d65);

            Entity tentacle3 = Entity.Resolve(0x0d65);

            Entity tentacle4 = Entity.Resolve(0x0d65);

            Entity tentacle5 = Entity.Resolve(0x0d65);

            Entity tentacle6 = Entity.Resolve(0x0d65);

            Entity tentacle7 = Entity.Resolve(0x0d65);

            Entity tentacle8 = Entity.Resolve(0x0d65);

            hermit.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(hermit);

            tentacle1.Move(pos.X + 9.5f, pos.Y + 15.5f);
            world.EnterWorld(tentacle1);

            tentacle2.Move(pos.X + 10.5f, pos.Y + 20.5f);
            world.EnterWorld(tentacle2);

            tentacle3.Move(pos.X + 15.5f, pos.Y + 21.5f);
            world.EnterWorld(tentacle3);

            tentacle4.Move(pos.X + 20.5f, pos.Y + 20.5f);
            world.EnterWorld(tentacle4);

            tentacle5.Move(pos.X + 21.5f, pos.Y + 15.5f);
            world.EnterWorld(tentacle5);

            tentacle6.Move(pos.X + 20.5f, pos.Y + 10.5f);
            world.EnterWorld(tentacle6);

            tentacle7.Move(pos.X + 15.5f, pos.Y + 9.5f);
            world.EnterWorld(tentacle7);

            tentacle8.Move(pos.X + 10.5f, pos.Y + 10.5f);
            world.EnterWorld(tentacle8);
        }
    }
}