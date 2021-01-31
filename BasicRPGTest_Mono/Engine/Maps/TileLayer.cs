using Microsoft.Xna.Framework;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class TileLayer
    {
        public Map map { get; set; }
        public string name { get; set; }
        public int index { get; set; }

        public Dictionary<Vector2, Tile> tiles = new Dictionary<Vector2, Tile>();
        public Tile[,] childTiles { get; set; }

        private Dictionary<Tile, List<Vector2>> v_MasterTiles = new Dictionary<Tile, List<Vector2>>();



        //====================================================================================
        // CONSTRUCTORS
        //====================================================================================
        /*
        public TileLayer(string name)
        {
            this.name = name;
        }

        public TileLayer(string name, int index)
        {
            this.name = name;
            this.index = index;
        }
        */

        public TileLayer(Map map, string name, int index)
        {
            this.map = map;
            this.name = name;
            this.index = index;

            // Create empty Array of Tiles with proper Dimensions
            childTiles = new Tile[map.width, map.height];
        }

        //====================================================================================
        // FUNCTIONS
        //====================================================================================

        // Investigate if these methods are based on memory pointers or the key object's data
        public void setTile(Vector2 pos, Tile tile)
        {
            tile.layer = this;
            tiles[pos] = tile;

            childTiles[(int)pos.X, (int)pos.Y] = tile;

            Tile test = childTiles[(int)pos.X, (int)pos.Y];

            // Set and store Parent along with Location
            Tile parent = tile.parent;

            if (parent != null)
            {
                // If this Tile Template doesn't exist in Master Tiles Dictionary yet... Add it.
                if (!v_MasterTiles.ContainsKey(tile.parent)) { v_MasterTiles.Add(tile.parent, new List<Vector2>()); }
                // Store Position of this Child Tile in Master (Template) Tile Dictionary
                v_MasterTiles[tile.parent].Add(tile.pos);
            }
        }

        public Tile getTile(Vector2 pos)
        {
            if (tiles.ContainsKey(pos))
                return tiles[pos];
            else
                return null;
        }

        public void clearTile(Vector2 pos)
        {
            tiles.Remove(pos);

            // Try to grab Tile again
            //Tile testTile = tiles[pos];



            //childTiles[(int)pos.X, (int)pos.Y] = null;
        }


        // Add Tile to Layer
        public bool addTile (Tile mTile)
        {
            // Check if Tile already exists at same Position and Layer
            if (tiles.ContainsKey(mTile.pos))
            {
                // Do NOT Add Tile to Map. A Tile already exists at that Position
                Utility.Util.myDebug(true, "TileLayer.cs addTile(Tile)", "Could NOT Add Tile. A Tile already exists at Layer(" + this.name + ") position: " + mTile.pos);
                return false;
            }
            // Otherwise...

            tiles.Add(mTile.pos, mTile);

            //this.childTiles[(int)mTile.pos.X, (int)mTile.pos.Y] = mTile;

            return true;
        }

    }
}
