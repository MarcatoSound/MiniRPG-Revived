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

        // A collection of tiles and their relative positions
        public Dictionary<Vector2, Tile> tiles;

        public Decoration(string name, int weight, Tile tile)
        {
            this.name = name;
            this.weight = weight;

            tiles = new Dictionary<Vector2, Tile>();
            tiles.Add(Vector2.Zero, tile);
        }
        public Decoration(string name, int weight, Dictionary<Vector2, Tile> tiles)
        {
            this.name = name;
            this.weight = weight;

            this.tiles = tiles;
        }
        public Decoration(string name, int weight, Decoration decoration)
        {
            this.name = name;
            this.weight = weight;

            this.tiles = decoration.tiles;
        }

        public void place(TileLayer layer, Vector2 decoPos, Biome biome)
        {
            Vector2 pos;
            Tile tile;
            Vector2 tilePos;
            foreach (KeyValuePair<Vector2, Tile> pair in tiles)
            {
                tile = pair.Value;
                pos = pair.Key;
                tilePos = decoPos;
                tilePos.X += pos.X;
                tilePos.Y += pos.Y;
                layer.setTile(tilePos, new Tile(tile, tilePos, biome));
            }
        }

    }
}
