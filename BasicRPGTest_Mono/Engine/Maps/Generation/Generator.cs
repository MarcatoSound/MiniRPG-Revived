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
using BasicRPGTest_Mono.Engine.Maps.Generation;

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

            NoiseMap biomeNoise = createBiomeNoise(seed);
            Biome biome;
            Biome[,] biomeTiles;

            NoiseMap noise = createLandNoise(seed, size);

            TileLayer waterLayer = new TileLayer("water");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    {
                        // Determine what biome this is.
                        if (biomeNoise[x, y] < 0)
                            biome = BiomeManager.getByName("field");
                        else if (biomeNoise[x, y] >= 0 && biomeNoise[x, y] < 0.5)
                            biome = BiomeManager.getByName("swamp");
                        else
                            biome = BiomeManager.getByName("desert");
                    }

                    if (noise[x, y] < 0)
                    {
                        waterLayer.setTile(new Vector2(x, y), new Tile(TileManager.getByName("water"), new Vector2(x, y), biome));
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
                    {
                        // Determine what biome this is.
                        if (biomeNoise[x, y] < -0.1)
                            biome = BiomeManager.getByName("field");
                        else if (biomeNoise[x, y] >= -0.1 && biomeNoise[x, y] < 0.5)
                            biome = BiomeManager.getByName("swamp");
                        else
                            biome = BiomeManager.getByName("desert");
                    }

                    if (noise[x, y] >= 0 && noise[x, y] < 0.07)
                    {
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.coastTile, new Vector2(x, y), biome));
                    }
                    else if (noise[x, y] >= 0)
                    {
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.groundTile, new Vector2(x, y), biome));
                    }
                }
            }
            layers.Add(groundLayer);

            TileLayer stoneLayer = new TileLayer("stone");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    {
                        // Determine what biome this is.
                        if (biomeNoise[x, y] < -0.1)
                            biome = BiomeManager.getByName("field");
                        else if (biomeNoise[x, y] >= -0.1 && biomeNoise[x, y] < 0.5)
                            biome = BiomeManager.getByName("swamp");
                        else
                            biome = BiomeManager.getByName("desert");
                    }

                    if (noise[x, y] > 0.5)
                    {
                        stoneLayer.setTile(new Vector2(x, y), new Tile(TileManager.getByName("stone"), new Vector2(x, y), biome));
                        continue;
                    }
                }
            }
            layers.Add(stoneLayer);

            TileLayer decoration = new TileLayer("decorations");
            Vector2 decoPos = new Vector2();
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    {
                        // Determine what biome this is.
                        if (biomeNoise[x, y] < -0.1)
                            biome = BiomeManager.getByName("field");
                        else if (biomeNoise[x, y] >= -0.1 && biomeNoise[x, y] < 0.5)
                            biome = BiomeManager.getByName("swamp");
                        else
                            biome = BiomeManager.getByName("desert");
                    }

                    if (biome.name.Equals("desert")) continue;

                    decoPos.X = x;
                    decoPos.Y = y;
                    if (decoration.tiles.ContainsKey(decoPos)) continue;
                    if (stoneLayer.tiles.ContainsKey(decoPos)) continue;
                    if (waterLayer.tiles.ContainsKey(decoPos)) continue;
                    if (groundLayer.getTile(decoPos) != null && groundLayer.getTile(decoPos).name.Equals("sand")) continue;

                    if (rand.Next(0, 100) > biome.decoChance) continue;

                    Decoration deco = biome.chooseDecoration();
                    deco.place(decoration, decoPos, biome);

                    //decoration.setTile(new Vector2(x, y), new Tile(TileManager.getByName("tree"), new Vector2(x, y), biome));

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
