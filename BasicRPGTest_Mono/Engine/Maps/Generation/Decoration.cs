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

        public Dictionary<Vector2, Tile> tiles;

        public Decoration(string name, int weight, Tile tile)
        {
            this.name = name;
            this.weight = weight;

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

    }
}
