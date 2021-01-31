using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class Region
    {
        Map map;
        
        // Region sizes should be in multiples of 4
        public const int regionSize = 16;

        public Vector2 pos { get; set; }
        public Vector2 regionPos { get; set; }
        public Rectangle box { get; set; }

        public List<Tile> tiles;
        public Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>> tilesCache = new Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>>();


        public Region(Map mMap, Vector2 pos, Vector2 regionPos)
        {
            this.map = mMap;
            this.pos = pos;
            this.regionPos = regionPos;
            box = new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), regionSize * TileManager.dimensions, regionSize * TileManager.dimensions);
            tiles = new List<Tile>();
        }

        public void addTiles(List<Tile> tiles)
        {
            this.tiles = tiles;
        }
        public void addTile(Tile tile)
        {
            this.tiles.Add(tile);
        }


        public void removeTile (Tile mTile)
        {
            // Remove Tile from Region
            tiles.Remove(mTile);

            // Remove Tile from Cache

            /*
            // Get TileLayer
            if (tilesCache.ContainsKey(mTile.layer))
            {
                Dictionary<Tile, List<Vector2>> tileTemplates = tilesCache[mTile.layer];
                List<Vector2> list = tileTemplates[mTile.parent];
                list.Remove(mTile.pos);

                if (list.Count < 1)
                {
                    // Remove TileTemplate from Cache
                    tileTemplates.Remove(mTile.parent);
                }
            }
            */
        }

        /*
        public void buildTileCache()
        {

            this.tilesCache.Clear();

            //Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>> tileLayer;
            Dictionary<Tile, List<Vector2>> tileLayer;
            List<Vector2> positions;

            // Create Parent Layer Object References as Keys
            foreach (TileLayer layer in map.layers)
            {
                tileLayer = new Dictionary<Tile, List<Vector2>>();
                tilesCache.Add(layer, tileLayer);
            }
            tileLayer = null;


            foreach (Tile tile in tiles)
            {
                if (tile.parent == null) continue;
                // Otherwise

                // Get Tile Layer
                tileLayer = tilesCache[tile.layer];
                positions = tileLayer[tile.parent];

                // If Tile Template does NOT exist in Cache yet
                if (positions == null)
                {
                    // Add it to Cache
                    tileLayer.Add(tile.parent, new List<Vector2>());
                }

                // Add this Tile's Position to List for this Tile Template
                positions.Add(tile.pos);
            }
        }
        */



        public void draw(SpriteBatch batch, TileLayer layer)
        {
            foreach (Tile tile in tiles)
            {
                if (tile.layer == layer)
                {
                    tile.drawAdjacentTiles(batch);
                }
            }
            batch.DrawRectangle(box, Color.White);
        }

    }
}
