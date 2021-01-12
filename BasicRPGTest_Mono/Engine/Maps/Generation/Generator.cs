using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using SharpNoise;
using SharpNoise.Modules;
using SharpNoise.Builders;

namespace BasicRPGTest_Mono.Engine
{
    public static class Generator
    {

        public static List<TileLayer> generateOverworldTiles(int size)
        {
            List<TileLayer> layers = new List<TileLayer>();

            Random rand = new Random();
            int id;


            Random seedGenerator = new Random();
            int seed = seedGenerator.Next(0, 9999999);
            System.Diagnostics.Debug.WriteLine($"Seed: {seed}");

            //NoiseMap biomeNoise = createBiomeNoise(seed);
            //Biome[,] biomeTiles;

            NoiseMap noise = createLandNoise(seed, size);

            TileLayer waterLayer = new TileLayer("water");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (noise[x, y] < 0)
                    {
                        waterLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(Convert.ToInt32(5)), new Vector2(x, y)));
                        continue;
                    }
                }
            }
            layers.Add(waterLayer);

            TileLayer groundLayer = new TileLayer("ground");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (noise[x, y] >= 0 && noise[x, y] < 0.07)
                    {
                        groundLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(Convert.ToInt32(3)), new Vector2(x, y)));
                    }
                    else if (noise[x, y] >= 0)
                    {
                        groundLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(Convert.ToInt32(0)), new Vector2(x, y)));
                    }
                }
            }
            layers.Add(groundLayer);

            TileLayer stoneLayer = new TileLayer("stone");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (noise[x, y] > 0.5)
                    {
                        stoneLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(Convert.ToInt32(2)), new Vector2(x, y)));
                        continue;
                    }
                }
            }
            layers.Add(stoneLayer);

            TileLayer decoration = new TileLayer("decorations");
            Vector2 treePos = new Vector2();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    treePos.X = x;
                    treePos.Y = y;
                    if (stoneLayer.tiles.ContainsKey(treePos)) continue;
                    if (waterLayer.tiles.ContainsKey(treePos)) continue;
                    if (rand.Next(0, 100) > 5) continue;
                    id = 4;

                    decoration.setTile(new Vector2(x, y), new Tile(TileManager.get(id), new Vector2(x, y)));

                }
            }
            layers.Add(decoration);

            return layers;
        }


        public static NoiseMap createBiomeNoise(int seed)
        {

            Cell gen1 = new Cell();
            gen1.Seed = seed;
            gen1.Frequency = 0.015;
            gen1.Type = Cell.CellType.Minkowsky;
        
            PlaneNoiseMapBuilder builder = new PlaneNoiseMapBuilder();
            builder.SourceModule = gen1;
            builder.SetBounds(0, 512, 0, 512);
            NoiseMap map = new NoiseMap(512, 512);
            builder.DestNoiseMap = map;
            builder.SetDestSize(512, 512);
            builder.Build();

            return map;
        }

        /// <summary>
        /// Generates perlin noise for determining the distribution of tile layers.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static NoiseMap createLandNoise(int seed, int size)
        {
            Perlin gen = new Perlin();
            gen.Seed = seed;
            gen.Frequency = 0.015;
            gen.Persistence = 0.5;
            gen.Lacunarity = 2.25;
            gen.OctaveCount = 4;

            ScaleBias mod = new ScaleBias();
            mod.Source0 = gen;
            mod.Bias = 0.2;
            mod.Scale = 0.68;

            PlaneNoiseMapBuilder builder = new PlaneNoiseMapBuilder();
            builder.SourceModule = mod;
            builder.SetBounds(0, size, 0, size);
            NoiseMap map = new NoiseMap(size, size);
            builder.DestNoiseMap = map;
            builder.SetDestSize(size, size);
            builder.Build();

            return map;
        }


    }
}
