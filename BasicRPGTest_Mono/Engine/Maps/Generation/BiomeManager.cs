﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps
{
    /// <summary>
    /// Stores and manages the game's loaded biomes. 
    /// NOTE: Does not contain data on the biomes present in a map.
    /// </summary>
    public static class BiomeManager
    {
        public static List<Biome> biomes;

        static BiomeManager()
        {
            biomes = new List<Biome>();
        }

        /// <summary>
        /// Adds and stores a biome in the loaded biome list.
        /// </summary>
        /// <param name="biome">The biome object that is being stored.</param>
        public static void add(Biome biome)
        {
            biomes.Add(biome);
        }

        /// <summary>
        /// Retrieves a biome object from the list of loaded biomes.
        /// </summary>
        /// <param name="index">The numerical index we are retrieving the biome from.</param>
        /// <returns>The biome matching the requested index.</returns>
        public static Biome get(int index)
        {
            if (index > biomes.Count - 1) return null;
            return biomes[index];
        }
        /// <summary>
        /// Retrieves a biome object from the list of loaded biomes using its namespace.
        /// </summary>
        /// <param name="name">The registered namespace of the biome.</param>
        /// <returns>The biome matching the "name" parameter provided. NULL if it wasn't found.</returns>
        public static Biome getByName(string name)
        {
            foreach (Biome biome in biomes)
            {
                if (biome.name == name) return biome;
            }

            return null;
        }

        public static List<Biome> getBiomes()
        {
            return biomes;
        }


        public static void Clear()
        {
            biomes.Clear();
        }
    }
}
