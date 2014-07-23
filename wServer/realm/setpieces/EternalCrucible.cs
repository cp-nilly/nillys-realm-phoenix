using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.logic.loot;
using terrain;
using db.data;

namespace wServer.realm.setpieces
{
    class GroundHeat1 : ISetPiece
    {
        public int Size { get { return 30; } }
        static readonly byte Heated = (byte)XmlDatas.IdToType["Hot Pebble"];

        Random rand = new Random();
        public void RenderSetPiece(World world, IntPoint pos)
        {
            int heatedRadius = 15;

            int[,] t = new int[Size, Size];

            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size / 2.0);
                    double dy = y - (Size / 2.0);
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= heatedRadius)
                        t[x, y] = 1;
                }


            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();

                        tile.TileId = Heated; tile.ObjType = 0;

                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;

                    }
                }
        }
    }

    class GroundHeat2 : ISetPiece
    {
        public int Size { get { return 30; } }
        static readonly byte Cooled = (byte)XmlDatas.IdToType["Scorch Blend"];

        Random rand = new Random();
        public void RenderSetPiece(World world, IntPoint pos)
        {
            int cooledRadius = 15;

            int[,] t = new int[Size, Size];

            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size / 2.0);
                    double dy = y - (Size / 2.0);
                    double r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= cooledRadius)
                        t[x, y] = 1;
                }


            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();

                        tile.TileId = Cooled; tile.ObjType = 0;

                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;

                    }
                }
        }
    }
}
