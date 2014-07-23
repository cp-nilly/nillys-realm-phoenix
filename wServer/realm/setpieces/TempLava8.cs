using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.logic.loot;
using terrain;

namespace wServer.realm.setpieces
{
    class TempLava8 : ISetPiece
    {
        public int Size { get { return 1; } }

        static readonly byte Floor = (byte)XmlDatas.IdToType["Red Quad"];

        Random rand = new Random();
        public void RenderSetPiece(World world, IntPoint pos)
        {
            int[,] t = new int[1, 1];



            t[0, 0] = 1;

            for (int x = 0; x < 1; x++)                    //Rendering
                for (int y = 0; y < 1; y++)
                {
                    if (t[x, y] == 1)
                    {

                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor; tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }
}
