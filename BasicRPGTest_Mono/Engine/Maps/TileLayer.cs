using Microsoft.Xna.Framework;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class TileLayer
    {
        public string name { get; set; }
        public Dictionary<Vector2, Tile> tiles;

        public TileLayer(string name)
        {
            this.name = name;
            tiles = new Dictionary<Vector2, Tile>();
        }

        // Investigate if these methods are based on memory pointers or the key object's data
        public void setTile(Vector2 pos, Tile tile)
        {
            tiles[pos] = tile;
        }
        public void clearTile(Vector2 pos)
        {
            tiles.Remove(pos);
        }
    }
}
