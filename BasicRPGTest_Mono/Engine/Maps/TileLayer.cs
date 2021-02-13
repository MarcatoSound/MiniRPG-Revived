using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

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
            if (tile == null) return;
            tile.layer = this;
        }
        public Tile getTile(Vector2 pos)
        {
            if (tiles.ContainsKey(pos))
                return tiles[pos];
            else
                return null;
        }
        public Tile getTile(float x, float y)
        {
            return getTile(new Vector2(x, y));
        }
        public void clearTile(Vector2 pos)
        {
            tiles.Remove(pos);
        }

        // Add Tile to Layer
        public bool addTile(Tile mTile)
        {
            // Check if Tile already exists at same Position and Layer
            if (tiles.ContainsKey(mTile.pos))
            {
                // Do NOT Add Tile to Map. A Tile already exists at that Position
                Util.myDebug(true, "TileLayer.cs addTile(Tile)", "Could NOT Add Tile. A Tile already exists at Layer(" + this.name + ") position: " + mTile.pos);
                return false;
            }
            // Otherwise...

            tiles.Add(mTile.pos, mTile);
            Console.WriteLine($"Added tile at: {mTile.pos}");

            //this.childTiles[(int)mTile.pos.X, (int)mTile.pos.Y] = mTile;

            return true;
        }


        public static implicit operator YamlSection(TileLayer tl)
        {
            YamlSection config = new YamlSection(tl.name);

            foreach (KeyValuePair<Vector2, Tile> pair in tl.tiles)
            {
                config.set($"tiles.{pair.Key.X}-{pair.Key.Y}", (YamlSection)pair.Value);
            }

            return config;
        }
    }
}
