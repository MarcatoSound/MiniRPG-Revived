using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.Maps.Generation;
using BasicRPGTest_Mono.Engine.Utility;
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
        public Tile stoneTile { get; set; }
        public Tile coastTile { get; set; }

        public List<Decoration> decorations { get; private set; } = new List<Decoration>();
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
        public Biome(DataPack pack, YamlSection config)
        {
            this.name = config.getName();
            string groundTileName = config.getString("ground_tile");
            string undergroundTileName = config.getString("underground_tile");
            string stoneTileName = config.getString("stone_tile");
            string coastTileName = config.getString("coast_tile");
            this.groundTile = TileManager.getByName(groundTileName);
            this.undergroundTile = TileManager.getByName(undergroundTileName);
            this.stoneTile = TileManager.getByName(stoneTileName);
            this.coastTile = TileManager.getByName(coastTileName);

            if (groundTile == null)
            {
                Console.WriteLine($"// ││└╾ ERR: Invalid ground tile '{groundTileName}'! Will use generator default...");
                return;
            }
            if (undergroundTile == null)
            {
                Console.WriteLine($"// ││└╾ ERR: Invalid underground tile '{undergroundTileName}'! Will use generator default...");
                return;
            }
            if (stoneTile == null)
            {
                Console.WriteLine($"// ││└╾ ERR: Invalid stone tile '{stoneTileName}'! Will use generator default...");
                return;
            }
            if (coastTile == null)
            {
                Console.WriteLine($"// ││└╾ ERR: Invalid coast tile '{coastTileName}'! Will use generator default...");
                return;
            }
        }


        public Decoration chooseDecoration()
        {
            if (decorations.Count < 1) return null;
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
