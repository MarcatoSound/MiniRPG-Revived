using Microsoft.Xna.Framework;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps.Generation
{
    public class Decoration
    {
        public string name { get; private set; }
        public int weight;
        public Rectangle box { get; private set; }

        // A collection of tiles and their relative positions
        public Dictionary<Vector2, Tile> tiles;
        
        // Eventually we will need decorations to support LAYERS

        public Decoration(string name, int weight, Tile tile)
        {
            this.name = name;
            this.weight = weight;

            tiles = new Dictionary<Vector2, Tile>();
            tiles.Add(Vector2.Zero, tile);
            box = new Rectangle(0, 0, TileManager.dimensions, TileManager.dimensions);
        }
        public Decoration(string name, int weight, Dictionary<Vector2, Tile> tiles)
        {
            this.name = name;
            this.weight = weight;

            this.tiles = tiles;
            box = new Rectangle(0, 0, TileManager.dimensions, TileManager.dimensions);
        }
        public Decoration(string name, int weight, Decoration decoration)
        {
            this.name = name;
            this.weight = weight;

            this.tiles = decoration.tiles;
            box = decoration.box;
        }

        public void place(List<TileLayer> layers, Vector2 decoPos, Biome biome)
        {
            Vector2 pos;
            Tile tile;
            Vector2 tilePos;
            TileLayer decoLayer = layers[3]; // TODO: Make this more flexible!

            foreach (KeyValuePair<Vector2, Tile> pair in tiles)
            {
                tile = pair.Value;
                if (tile == null) continue;
                pos = pair.Key;
                tilePos = decoPos;
                tilePos.X += pos.X;
                tilePos.Y += pos.Y;

                Tile targetTile;
                foreach (TileLayer layer in layers)
                {
                    targetTile = layer.getTile(tilePos);
                    if (targetTile == null) continue;
                    if (targetTile.isCollidable) goto TILES; 
                }

                if (decoLayer.tiles.ContainsKey(tilePos)) return;

                decoLayer.setTile(tilePos, new Tile(tile, tilePos, biome));

            TILES:;

            }

        }

    }
}
