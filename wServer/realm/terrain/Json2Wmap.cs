#region

using System;
using System.Collections.Generic;
using System.IO;
using db.data;
using Ionic.Zlib;
using Newtonsoft.Json;

#endregion

namespace terrain
{
    public class Json2Wmap
    {
        public static void Convert(string from, string to)
        {
            var x = Convert(File.ReadAllText(from));
            File.WriteAllBytes(to, x);
        }

        public static byte[] Convert(string json)
        {
            var obj = JsonConvert.DeserializeObject<json_dat>(json);
            var dat = ZlibStream.UncompressBuffer(obj.data);

            var tileDict = new Dictionary<short, TerrainTile>();
            for (var i = 0; i < obj.dict.Length; i++)
            {
                var o = obj.dict[i];
                tileDict[(short) i] = new TerrainTile
                {
                    TileId = o.ground == null ? (short) 0xff : XmlDatas.IdToType[o.ground],
                    TileObj = o.objs == null ? null : o.objs[0].id,
                    Name = o.objs == null ? "" : o.objs[0].name ?? "",
                    Terrain = TerrainType.None,
                    Region =
                        o.regions == null
                            ? TileRegion.None
                            : (TileRegion) Enum.Parse(typeof (TileRegion), o.regions[0].id.Replace(' ', '_'))
                };
            }

            var tiles = new TerrainTile[obj.width, obj.height];
            using (var rdr = new NReader(new MemoryStream(dat)))
                for (var y = 0; y < obj.height; y++)
                    for (var x = 0; x < obj.width; x++)
                    {
                        tiles[x, y] = tileDict[rdr.ReadInt16()];
                    }
            return WorldMapExporter.Export(tiles);
        }

        public static byte[] ConvertMakeWalls(string json)
        {
            var obj = JsonConvert.DeserializeObject<json_dat>(json);
            var dat = ZlibStream.UncompressBuffer(obj.data);

            var tileDict = new Dictionary<short, TerrainTile>();
            for (var i = 0; i < obj.dict.Length; i++)
            {
                var o = obj.dict[i];
                tileDict[(short) i] = new TerrainTile
                {
                    TileId = o.ground == null ? (short) 0xff : XmlDatas.IdToType[o.ground],
                    TileObj = o.objs == null ? null : o.objs[0].id,
                    Name = o.objs == null ? "" : o.objs[0].name ?? "",
                    Terrain = TerrainType.None,
                    Region =
                        o.regions == null
                            ? TileRegion.None
                            : (TileRegion) Enum.Parse(typeof (TileRegion), o.regions[0].id.Replace(' ', '_'))
                };
            }

            var tiles = new TerrainTile[obj.width, obj.height];
            using (var rdr = new NReader(new MemoryStream(dat)))
                for (var y = 0; y < obj.height; y++)
                    for (var x = 0; x < obj.width; x++)
                    {
                        tiles[x, y] = tileDict[rdr.ReadInt16()];
                        tiles[x, y].X = x;
                        tiles[x, y].Y = y;
                    }

            foreach (var i in tiles)
            {
                if (i.TileId == 0xff && i.TileObj == null)
                {
                    var createWall = false;
                    for (var ty = -1; ty <= 1; ty++)
                        for (var tx = -1; tx <= 1; tx++)
                            try
                            {
                                if (tiles[i.X + tx, i.Y + ty].TileId != 0xff)
                                    createWall = true;
                            }
                            catch
                            {
                            }
                    if (createWall)
                        tiles[i.X, i.Y].TileObj = "Grey Wall";
                }
            }

            return WorldMapExporter.Export(tiles);
        }

        // ------------ Convert to UDL format ------------- //
        public static byte[] ConvertUDL(string json)
        {
            var obj = JsonConvert.DeserializeObject<json_dat>(json);
            var dat = ZlibStream.UncompressBuffer(obj.data);

            var rand = new Random();

            var tileDict = new Dictionary<short, TerrainTile>();
            for (var i = 0; i < obj.dict.Length; i++)
            {
                var o = obj.dict[i];
                tileDict[(short) i] = new TerrainTile
                {
                    TileId = o.ground == null ? (short) 0xff : XmlDatas.IdToType[o.ground],
                    TileObj = o.objs == null ? null : o.objs[0].id,
                    Name = o.objs == null ? "" : o.objs[0].name ?? "",
                    Terrain = TerrainType.None,
                    Region =
                        o.regions == null
                            ? TileRegion.None
                            : (TileRegion) Enum.Parse(typeof (TileRegion), o.regions[0].id.Replace(' ', '_'))
                };
            }

            var tiles = new TerrainTile[obj.width, obj.height];
            using (var rdr = new NReader(new MemoryStream(dat)))
                for (var y = 0; y < obj.height; y++)
                    for (var x = 0; x < obj.width; x++)
                    {
                        tiles[x, y] = tileDict[rdr.ReadInt16()];
                        tiles[x, y].X = x;
                        tiles[x, y].Y = y;
                    }

            foreach (var i in tiles)
            {
                if (i.TileId == 0xff && i.TileObj == null)
                {
                    var createWall = false;
                    for (var ty = -1; ty <= 1; ty++)
                        for (var tx = -1; tx <= 1; tx++)
                            try
                            {
                                if (tiles[i.X + tx, i.Y + ty].TileId != 0xff && tiles[i.X + tx, i.Y + ty].TileId != 0xfe &&
                                    tiles[i.X + tx, i.Y + ty].TileId != 0xfd && tiles[i.X + tx, i.Y + ty].TileId != 0xe8)
                                    createWall = true;
                            }
                            catch
                            {
                            }
                    if (createWall)
                        tiles[i.X, i.Y].TileObj = rand.Next(1, 5) == 1 ? "Grey Torch Wall" : "Grey Wall";
                }
                else if (i.TileId == XmlDatas.IdToType["Grey Closed"] && rand.Next(1, 4) == 1)
                {
                    tiles[i.X, i.Y].TileId = XmlDatas.IdToType["Grey Quad"];
                }
            }

            return WorldMapExporter.Export(tiles);
        }

        private struct json_dat
        {
            public byte[] data;
            public loc[] dict;
            public int height;
            public int width;
        }

        private struct loc
        {
            public string ground;
            public obj[] objs;
            public obj[] regions;
        }

        private struct obj
        {
            public string id;
            public string name;
        }
    }
}