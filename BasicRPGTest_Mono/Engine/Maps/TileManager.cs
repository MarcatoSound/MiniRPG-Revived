using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public static class TileManager
    {
        public static List<Tile> tiles { get; set; }
        public static int dimensions { get; set; }

        static TileManager()
        {
            tiles = new List<Tile>();
            dimensions = 32;
        }

        public static void add(Tile tile)
        {
            tiles.Add(tile);
        }

        public static Tile get(int i)
        {
            if (i > tiles.Count - 1) return null;
            return tiles[i];
        }

        public static List<Tile> getTiles()
        {
            return tiles;
        }

        public static void Clear()
        {
            tiles.Clear();
        }
    }
}
