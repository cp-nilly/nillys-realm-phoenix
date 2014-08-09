#region

using System;
using common.data;

#endregion

namespace wServer.realm.setpieces
{
    internal class TempLava1 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte ScorchBlend = (byte) XmlDatas.IdToType["Scorch Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 1; }
        }


        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[1, 1];


            t[0, 0] = 1;

            for (int x = 0; x < 1; x++) //Rendering
                for (int y = 0; y < 1; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava2 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 3; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[3, 3];


            t[0, 1] = 1;
            t[1, 1] = 1;
            t[2, 1] = 1;
            t[1, 0] = 1;
            t[1, 2] = 1;

            for (int x = 0; x < 3; x++) //Rendering
                for (int y = 0; y < 3; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava3 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[5, 5];


            t[0, 2] = 1;
            t[1, 2] = 1;
            t[2, 2] = 1;
            t[3, 2] = 1;
            t[4, 2] = 1;
            t[2, 0] = 1;
            t[2, 1] = 1;
            t[2, 3] = 1;
            t[2, 4] = 1;
            t[1, 1] = 1;
            t[1, 3] = 1;
            t[3, 3] = 1;
            t[3, 1] = 1;


            for (int x = 0; x < 5; x++) //Rendering
                for (int y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava4 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];


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


            for (int x = 0; x < 5; x++) //Rendering
                for (int y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava5 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Red Quad"];


        private Random rand = new Random();

        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[5, 5];

            t[0, 1] = 2;
            t[0, 2] = 1;
            t[0, 3] = 2;
            t[1, 2] = 1;
            t[2, 2] = 1;
            t[3, 2] = 1;
            t[4, 1] = 2;
            t[4, 2] = 1;
            t[4, 3] = 2;
            t[2, 0] = 1;
            t[2, 1] = 1;
            t[2, 3] = 1;
            t[2, 4] = 1;
            t[1, 1] = 1;
            t[1, 3] = 1;
            t[3, 3] = 1;
            t[3, 1] = 1;
            t[1, 0] = 2;
            t[3, 0] = 2;
            t[1, 4] = 2;
            t[3, 4] = 2;


            for (int x = 0; x < 5; x++) //Rendering
                for (int y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava6 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Red Quad"];

        private Random rand = new Random();

        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[5, 5];


            t[0, 2] = 2;
            t[1, 2] = 1;
            t[2, 2] = 1;
            t[3, 2] = 1;
            t[4, 2] = 2;
            t[2, 0] = 2;
            t[2, 1] = 1;
            t[2, 3] = 1;
            t[2, 4] = 2;
            t[1, 1] = 2;
            t[1, 3] = 2;
            t[3, 3] = 2;
            t[3, 1] = 2;


            for (int x = 0; x < 5; x++) //Rendering
                for (int y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava7 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Red Quad"];

        private Random rand = new Random();

        public int Size
        {
            get { return 3; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[3, 3];


            t[0, 1] = 2;
            t[1, 1] = 1;
            t[2, 1] = 2;
            t[1, 0] = 2;
            t[1, 2] = 2;

            for (int x = 0; x < 3; x++) //Rendering
                for (int y = 0; y < 3; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava8 : ISetPiece
    {
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Red Quad"];

        private Random rand = new Random();

        public int Size
        {
            get { return 1; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[1, 1];


            t[0, 0] = 1;

            for (int x = 0; x < 1; x++) //Rendering
                for (int y = 0; y < 1; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava9 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Scorch Blend"];


        private Random rand = new Random();

        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[5, 5];

            t[0, 1] = 2;
            t[0, 2] = 1;
            t[0, 3] = 2;
            t[1, 2] = 1;
            t[2, 2] = 1;
            t[3, 2] = 1;
            t[4, 1] = 2;
            t[4, 2] = 1;
            t[4, 3] = 2;
            t[2, 0] = 1;
            t[2, 1] = 1;
            t[2, 3] = 1;
            t[2, 4] = 1;
            t[1, 1] = 1;
            t[1, 3] = 1;
            t[3, 3] = 1;
            t[3, 1] = 1;
            t[1, 0] = 2;
            t[3, 0] = 2;
            t[1, 4] = 2;
            t[3, 4] = 2;


            for (int x = 0; x < 5; x++) //Rendering
                for (int y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava10 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Scorch Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 5; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[5, 5];


            t[0, 2] = 2;
            t[1, 2] = 1;
            t[2, 2] = 1;
            t[3, 2] = 1;
            t[4, 2] = 2;
            t[2, 0] = 2;
            t[2, 1] = 1;
            t[2, 3] = 1;
            t[2, 4] = 2;
            t[1, 1] = 2;
            t[1, 3] = 2;
            t[3, 3] = 2;
            t[3, 1] = 2;


            for (int x = 0; x < 5; x++) //Rendering
                for (int y = 0; y < 5; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava11 : ISetPiece
    {
        private static readonly byte Lava = (byte) XmlDatas.IdToType["Lava Blend"];
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Scorch Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 3; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[3, 3];


            t[0, 1] = 2;
            t[1, 1] = 1;
            t[2, 1] = 2;
            t[1, 0] = 2;
            t[1, 2] = 2;

            for (int x = 0; x < 3; x++) //Rendering
                for (int y = 0; y < 3; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    if (t[x, y] == 2)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }

    internal class TempLava12 : ISetPiece
    {
        private static readonly byte Floor = (byte) XmlDatas.IdToType["Scorch Blend"];

        private Random rand = new Random();

        public int Size
        {
            get { return 1; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var t = new int[1, 1];


            t[0, 0] = 1;

            for (int x = 0; x < 1; x++) //Rendering
                for (int y = 0; y < 1; y++)
                {
                    if (t[x, y] == 1)
                    {
                        WmapTile tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Floor;
                        tile.ObjType = 0;
                        if (world.Obstacles[x + pos.X, y + pos.Y] == 0)
                            world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }
        }
    }
}