using System;
using db.data;

namespace wServer.realm.setpieces
{
    internal class GroundHeat1 : ISetPiece
    {
        private static readonly byte Heated = (byte) XmlDatas.IdToType["Hot Pebble"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            int heatedRadius = 15;

            var t = new int[Size, Size];

            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= heatedRadius)
                        t[x, y] = 1;
                }


            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();

                        tile.TileId = Heated;
                        tile.ObjType = 0;

                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class GroundHeat2 : ISetPiece
    {
        private static readonly byte Cooled = (byte) XmlDatas.IdToType["Scorch Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            int cooledRadius = 15;

            var t = new int[Size, Size];

            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    double dx = x - (Size/2.0);
                    double dy = y - (Size/2.0);
                    double r = Math.Sqrt(dx*dx + dy*dy);
                    if (r <= cooledRadius)
                        t[x, y] = 1;
                }


            for (int x = 0; x < Size; x++)
                for (int y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();

                        tile.TileId = Cooled;
                        tile.ObjType = 0;

                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }
}