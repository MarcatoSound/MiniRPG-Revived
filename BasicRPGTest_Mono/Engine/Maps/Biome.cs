using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class Biome
    {
        public string name;
        public Tile groundTile;
        public Tile stoneTile;

        public List<Tile> decorations;

        public Biome(string name, Tile groundTile, Tile stoneTile)
        {
            this.name = name;
            this.groundTile = groundTile;
            this.stoneTile = stoneTile;

            decorations = new List<Tile>();
        }
        public Biome(string name, Tile groundTile, Tile stoneTile, List<Tile> decorations)
        {
            this.name = name;
            this.groundTile = groundTile;
            this.stoneTile = stoneTile;

            this.decorations = decorations;
        }
    }
}
