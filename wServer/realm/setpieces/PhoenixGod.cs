#region

using System;
using System.Collections.Generic;
using db.data;

#endregion

namespace wServer.realm.setpieces
{
    internal class PhoenixGod : ISetPiece
    {
        private static readonly byte ScorchBlend = (byte)XmlDatas.IdToType["Scorch Blend"];
        private static readonly byte GreyQuad = (byte)XmlDatas.IdToType["Grey Quad2"];
        private static readonly byte Lava = (byte)XmlDatas.IdToType["Lava Blend"];
        protected static readonly short BrokenPillar = XmlDatas.IdToType["Broken Grey Pillar"];
        protected static readonly short Pillar = XmlDatas.IdToType["Grey Pillar"];


        private readonly Random rand = new Random();

        public int Size
        {
            get { return 30; }
        }

        public void RenderSetPiece(World world, IntPoint pos)
        {
            var lavaRadius = 13;
            var greyQuadRadius = 4f;
            var scorchBlendRadius = 11f;

            var border = new List<IntPoint>();

            var o = new int[Size, Size];
            var t = new int[Size, Size];

            for (var y = 0; y < Size; y++) //Lava
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - (Size / 2.0);
                    var dy = y - (Size / 2.0);
                    var r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= lavaRadius + rand.NextDouble() * 3)
                    {
                        t[x, y] = 1;
                    }
                }
            for (var y = 0; y < Size; y++) //ScorchBlend
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - (Size / 2.0);
                    var dy = y - (Size / 2.0);
                    var r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= scorchBlendRadius)
                        t[x, y] = 2;
                }
            for (var y = 0; y < Size; y++) //GreyQuad
                for (var x = 0; x < Size; x++)
                {
                    var dx = x - (Size / 2.0);
                    var dy = y - (Size / 2.0);
                    var r = Math.Sqrt(dx * dx + dy * dy);
                    if (r <= greyQuadRadius)
                    {
                        t[x, y] = 3;
                    }
                }
            
            t[24, (int)6] = rand.Next(4, 7);
            t[(int)6, 24] = rand.Next(4, 7);
            t[24, 24] = rand.Next(4, 7);
            t[(int)6, (int)6] = rand.Next(4, 7);

            t[14, 0] = 2;
            t[15, 0] = 2;
            t[16, 0] = 2;
            t[14, 1] = 2;
            t[15, 1] = 2;
            t[16, 1] = 2;
            t[14, 2] = 2;
            t[15, 2] = 2;
            t[16, 2] = 2;
            t[14, 3] = 2;
            t[15, 3] = 2;
            t[16, 3] = 2;
            t[14, 4] = 2;
            t[15, 4] = 2;
            t[16, 4] = 2;
            t[14, 5] = 2;
            t[15, 5] = 2;
            t[16, 5] = 2;
                

            for (var x = 0; x < Size; x++)
                for (var y = 0; y < Size; y++)
                {
                    if (t[x, y] == 1)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 2)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = ScorchBlend;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 3)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = GreyQuad;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 4)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = GreyQuad;
                        tile.ObjType = 0;
                        world.Obstacles[x + pos.X, y + pos.Y] = 0;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                    else if (t[x, y] == 4 || t[x, y] == 5)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = BrokenPillar;
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }

                    else if (t[x, y] == 6)
                    {
                        var tile = world.Map[x + pos.X, y + pos.Y].Clone();
                        tile.TileId = Lava;
                        tile.ObjType = Pillar;
                        if (tile.ObjId == 0) tile.ObjId = world.GetNextEntityId();
                        world.Obstacles[x + pos.X, y + pos.Y] = 2;
                        world.Map[x + pos.X, y + pos.Y] = tile;
                    }
                }


            var Phoenix = Entity.Resolve(0x2f62);
            Phoenix.Move(pos.X + 15.5f, pos.Y + 15.5f);
            world.EnterWorld(Phoenix);

        }
    }
}