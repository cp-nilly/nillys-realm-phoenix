#region

using System;

#endregion

namespace terrain
{
    internal enum TerrainType
    {
        None,
        Mountains,
        HighSand,
        HighPlains,
        HighForest,
        MidSand,
        MidPlains,
        MidForest,
        LowSand,
        LowPlains,
        LowForest,
        ShoreSand,
        ShorePlains,
    }

    internal enum TileRegion : byte
    {
        None,
        Spawn,
        Realm_Portals,
        Store_1,
        Store_2,
        Store_3,
        Store_4,
        Store_5,
        Store_6,
        Store_7,
        Store_8,
        Store_9,
        Store_10,
        Vault,
        Loot,
        Defender,
        Hallway,
        Hallway_1,
        Hallway_2,
        Hallway_3,
        Enemy,
        Setpiece1,
        Setpiece2,
        Setpiece3
    }

    internal struct TerrainTile : IEquatable<TerrainTile>
    {
        public string Biome;
        public byte Elevation;
        public float Moisture;
        public string Name;
        public int PolygonId;
        public TileRegion Region;
        public TerrainType Terrain;
        public short TileId;
        public string TileObj;
        public int X;
        public int Y;

        public bool Equals(TerrainTile other)
        {
            return
                TileId == other.TileId &&
                TileObj == other.TileObj &&
                Name == other.Name &&
                Terrain == other.Terrain &&
                Region == other.Region;
        }
    }
}