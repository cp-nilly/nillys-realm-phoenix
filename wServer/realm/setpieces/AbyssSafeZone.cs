#region

using System;
using db.data;

#endregion

namespace wServer.realm.setpieces
{
    internal class AbyssSafeZone : ISetPiece
    {
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Red Checker Board"];


        private Random rand = new Random();

        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[5, 5];

            t[0, 1] = 1;
            t[0, 2] = 1;
            t[0, 3] = 1;
            t[1, 2] = 1;
            t[2, 2] = 1;
            t[3, 2] = 1;
            t[4, 1] = 1;
            t[4, 2] = 1;
            t[4, 3] = 1;
            t[2, 0] = 1;
            t[2, 1] = 1;
            t[2, 3] = 1;
            t[2, 4] = 1;
            t[1, 1] = 1;
            t[1, 3] = 1;
            t[3, 3] = 1;
            t[3, 1] = 1;
            t[1, 0] = 1;
            t[3, 0] = 1;
            t[1, 4] = 1;
            t[3, 4] = 1;


            for (var x = 0; x < 5; x++) //Rendering
                for (var y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
            var portal = Entity.Resolve(0x1733);
            portal.Move(pos.X + 2.5f, pos.Y + 2.5f);
            world.EnterWorld(portal);
        }
    }
}