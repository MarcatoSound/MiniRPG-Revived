using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public class Biome
    {
        public string name { get; private set; }
        public Tile groundTile { get; set; }
        public Tile undergroundTile { get; set; }
        public Tile coastTile { get; set; }

        public List<Tile> decorations { get; private set; }


        public Biome(string name, Tile groundTile)
        {
            this.name = name;
            this.groundTile = groundTile;

            decorations = new List<Tile>();
        }
        public Biome(string name, Tile groundTile, List<Tile> decorations)
        {
            this.name = name;
            this.groundTile = groundTile;

            this.decorations = decorations;
        }



    }
}
