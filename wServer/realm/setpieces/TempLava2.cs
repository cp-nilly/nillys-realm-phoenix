using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.logic.loot;
using terrain;

namespace wServer.realm.setpieces
{
    class TempLava2 : ISetPiece
    {
        public int Size { get { return 3; } }

        static readonly byte Lava = (byte)XmlDatas.IdToType["Lava Blend"];

        Random rand = new Random();
        public void RenderSetPiece(World world, IntPoint pos)
        {
            int[,] t = new int[3, 3];

            
                        t[0, 1] = 1;
                        t[1, 1] = 1;
                        t[2, 1] = 1;
                        t[1, 0] = 1;
                        t[1, 2] = 1;
                
            for (int x = 0; x < 3; x++)                    //Rendering
                for (int y = 0; y < 3; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava; tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }
}
