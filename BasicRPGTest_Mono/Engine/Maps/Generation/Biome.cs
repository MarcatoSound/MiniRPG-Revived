using BasicRPGTest_Mono.Engine.Maps.Generation;
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

        public List<Decoration> decorations { get; private set; }
        public int decoChance { get; set; }


        public Biome(string name, Tile groundTile)
        {
            this.name = name;
            this.groundTile = groundTile;

            decorations = new List<Decoration>();
            decorations.Add(new Decoration("Blank", 100, tile: null));
            decoChance = 1;
        }
        public Biome(string name, Tile groundTile, List<Decoration> decorations)
        {
            this.name = name;
            this.groundTile = groundTile;

            this.decorations = decorations;
            decorations.Add(new Decoration("Blank", 100, tile: null));
            decoChance = 1;
        }


        public Decoration chooseDecoration()
        {
            // O(n) performance
            int totalRatio = 0;

            foreach (Decoration d in decorations)
                totalRatio += d.weight;

            Random random = new Random();
            int x = random.Next(0, totalRatio);

            int iteration = 0; // so you know what to do next
            foreach (Decoration d in decorations)
            {
                if ((x -= d.weight) < 0)
                    break;
                iteration++;
            }

            return decorations[iteration];
        }

    }
}
