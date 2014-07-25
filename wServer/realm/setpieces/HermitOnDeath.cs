#region

using System;
using System.Collections.Generic;
using db.data;

#endregion

namespace wServer.realm.setpieces
{
    internal class HermitOnDeath1 : ISetPiece
    {
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            float deepWaterRadius = 1f;

            var border = new List<IntPoint>();

            var t = new int[Size, Size];


            for (int y = 0; y < Size; y++) //Replace Deep Water With NWater
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= deepWaterRadius)
                    {
                        t[x, y] = 1;
                    }
                }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class HermitOnDeath2 : ISetPiece
    {
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            float deepWaterRadius = 2f;

            var border = new List<IntPoint>();

            var t = new int[Size, Size];


            for (int y = 0; y < Size; y++) //Replace Deep Water With NWater
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= deepWaterRadius)
                    {
                        t[x, y] = 1;
                    }
                }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class HermitOnDeath3 : ISetPiece
    {
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            float deepWaterRadius = 3f;

            var border = new List<IntPoint>();

            var t = new int[Size, Size];


            for (int y = 0; y < Size; y++) //Replace Deep Water With NWater
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= deepWaterRadius)
                    {
                        t[x, y] = 1;
                    }
                }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class HermitOnDeath4 : ISetPiece
    {
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            float deepWaterRadius = 4.5f;

            var border = new List<IntPoint>();

            var t = new int[Size, Size];


            for (int y = 0; y < Size; y++) //Replace Deep Water With NWater
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= deepWaterRadius)
                    {
                        t[x, y] = 1;
                    }
                }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class HermitOnDeath5 : ISetPiece
    {
        private static readonly byte Water = (byte) XmlDatas.IdToType["Shallow Water"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            float deepWaterRadius = 6f;

            var border = new List<IntPoint>();

            var t = new int[Size, Size];


            for (int y = 0; y < Size; y++) //Replace Deep Water With NWater
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= deepWaterRadius)
                    {
                        t[x, y] = 1;
                    }
                }
            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Water;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }
}