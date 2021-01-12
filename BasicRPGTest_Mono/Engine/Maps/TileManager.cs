using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    /// <summary>
    /// Stores and manages the game's template/parent tile pool. 
    /// NOTE: Does not contain data about the distribution of tiles on a map!
    /// </summary>
    public static class TileManager
    {
        public static List<Tile> tiles { get; set; }
        public static int dimensions { get; set; }

        static TileManager()
        {
            tiles = new List<Tile>();
            dimensions = 32;
        }
        /// <summary>
        /// Adds and stores a tile in the pool of loaded template tiles.
        /// </summary>
        /// <param name="tile">The parent tile object that is being stored.</param>
        public static void add(Tile tile)
        {
            tiles.Add(tile);
        }

        /// <summary>
        /// Retrieves a parent tile object from the list of loaded tiles.
        /// </summary>
        /// <param name="index">The numerical index we are retrieving the tile from.</param>
        /// <returns>The parent tile matching the requested index.</returns>
        public static Tile get(int index)
        {
            if (index > tiles.Count - 1) return null;
            return tiles[index];
        }
        /// <summary>
        /// Retrieves a parent tile object from the list of loaded tiles using its namespace.
        /// </summary>
        /// <param name="name">The registered namespace of the tile.</param>
        /// <returns>The parent tile matching the "name" parameter provided. NULL if it wasn't found.</returns>
        public static Tile getByName(string name)
        {
            foreach (Tile tile in tiles)
            {
                if (tile.name == name) return tile;
            }

            return null;
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
