using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    public static class BiomeManager
    {
        public static List<Biome> biomes;

        static BiomeManager()
        {
            biomes = new List<Biome>();
        }

        public static void add(Biome biome)
        {
            biomes.Add(biome);
        }

        public static Biome get(int i)
        {
            if (i > biomes.Count - 1) return null;
            return biomes[i];
        }
        public static Biome getByName(string name)
        {
            foreach (Biome biome in biomes)
            {
                if (biome.name == name) return biome;
            }

            return null;
        }

        public static List<Biome> getWindows()
        {
            return biomes;
        }


        public static void Clear()
        {
            biomes.Clear();
        }
    }
}
