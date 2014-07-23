#region

using System;
using System.Collections.Generic;
using db.data;

#endregion

namespace wServer.realm.setpieces
{
    internal class BeachBum : ISetPiece
    {
        private static readonly byte Island = (byte) XmlDatas.IdToType["Ghost Water Beach"];
        private static readonly byte Water = (byte) XmlDatas.IdToType["GhostWater"];
        private static readonly byte Outer = (byte) XmlDatas.IdToType["Ghost Water Beach"];
        private static readonly short Tree = XmlDatas.IdToType["Palm Tree"];

        private readonly Random rand = new Random();

        public int Size
        {
            get { return 42; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            double sandRadius = 20;
            var waterRadius = 18;
            var islandRadius = 2.5f;
            var TreeCount = 0;

            var border = new List<IntPoint>();

            var t = new int[Size, Size];
            for (var y = 0; y < Size; y++) //Outer
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - (Size/2.0);
                    var dy = y - (Size/2.0);
                    sandRadius = 20 + rand.Next(0, 3);
                    var r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= sandRadius)
                    {
                        t[x, y] = 1;
                    }
                }
            for (var y = 0; y < Size; y++) //Water
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - (Size/2.0);
                    var dy = y - (Size/2.0);
                    var r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= waterRadius)
                    {
                        t[x, y] = 2;
                    }
                }

            for (var y = 0; y < Size; y++) //Island
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - (Size/2.0);
                    var dy = y - (Size/2.0);
                    var r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= islandRadius)
                    {
                        var Tree = rand.Next(1, 4);

                        t[x, y] = 3;
                        if (Tree == 1 && x != 21 && y != 21 && TreeCount < 3)
                        {
                            t[x, y] = 4;
                            TreeCount++;
                        }
                    }
                }
            for (var x = 0; x < Size; x++)
                for (var y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Outer;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Island;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 4)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Island;
                        tile.ObjType = Tree;
                        tile.Name = "size:" + (rand.Next()%2 == 0 ? 120 : 140);
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
            var Bum = Entity.Resolve(0x0e55);
            Bum.Move(pos.X + 21.5f, pos.Y + 21.5f);
            world.EnterWorld(Bum);
        }
    }
}