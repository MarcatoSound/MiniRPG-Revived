using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using RPGEngine;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine.Maps.Generation
{
    public class Generator
    {

        public string name { get; private set; }

        public double waterLevel { get; private set; }
        public double sandLevel { get; private set; }
        public double stoneLevel { get; private set; }
        public double biomeSize { get; private set; }

        public double scale { get; private set; }
        public int roughness { get; private set; }
        public double depth { get; private set; }

        public Generator(DataPack pack, YamlSection config)
        {
            name = config.getName();

            waterLevel = (config.getDouble("water_frequency", 0.5) * 2) - 1;
            sandLevel = waterLevel + 0.07;
            stoneLevel = 1 - (config.getDouble("stone_frequency", 0.25) * 2);

            biomeSize = config.getDouble("biome_size", 1);

            scale = config.getDouble("advanced.scale", 1);
            roughness = config.getInt("advanced.roughness", 4);
            depth = config.getDouble("advanced.depth", 0.2);

        }

        public List<TileLayer> generateLayers(int size)
        {

            DataPackManager.mapTileCount = 0;
            DataPackManager.maxLayerTiles = size * size;
            DataPackManager.mapTotalTiles = DataPackManager.maxLayerTiles;

            List<TileLayer> layers = new List<TileLayer>();

            Random rand = new Random();
            Random seedGenerator = new Random();
            int seed = seedGenerator.Next(0, 9999999);
            System.Diagnostics.Debug.WriteLine($"Seed: {seed}");

            NoiseMap biomeNoise = createBiomeNoise(seed);
            Biome biome;

            NoiseMap noise = createLandNoise(seed, size);


            TileLayer waterLayer = new TileLayer("water");
            TileLayer groundLayer = new TileLayer("ground");
            TileLayer stoneLayer = new TileLayer("stone");
            TileLayer decorations = new TileLayer("decorations");
            layers.Add(waterLayer);
            layers.Add(groundLayer);
            layers.Add(stoneLayer);
            layers.Add(decorations);

            Vector2 decoPos = new Vector2();

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    DataPackManager.mapTileCount++;
                    {
                        // Determine what biome this is.
                        if (biomeNoise[x, y] < 0)
                            biome = BiomeManager.getByName("field");
                        else if (biomeNoise[x, y] >= 0 && biomeNoise[x, y] < 0.5)
                            biome = BiomeManager.getByName("swamp");
                        else
                            biome = BiomeManager.getByName("desert");
                    }

                    // Water layer
                    if (noise[x, y] < waterLevel)
                        waterLayer.setTile(new Vector2(x, y), new Tile(TileManager.getByName("water"), new Vector2(x, y), biome));

                    // Ground layer
                    if (noise[x, y] >= waterLevel && noise[x, y] < sandLevel)
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.coastTile, new Vector2(x, y), biome));
                    else if (noise[x, y] >= sandLevel && noise[x, y] <= stoneLevel)
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.groundTile, new Vector2(x, y), biome));
                    else if (noise[x, y] > stoneLevel)
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.undergroundTile, new Vector2(x, y), biome));

                    // Stone layer
                    if (noise[x, y] > stoneLevel)
                        stoneLayer.setTile(new Vector2(x, y), new Tile(TileManager.getByName("stone"), new Vector2(x, y), biome));

                    // Decorations layer
                    if (biome.name.Equals("desert")) continue;

                    decoPos.X = x;
                    decoPos.Y = y;
                    if (decorations.tiles.ContainsKey(decoPos)) continue;
                    if (stoneLayer.tiles.ContainsKey(decoPos)) continue;
                    if (waterLayer.tiles.ContainsKey(decoPos)) continue;
                    if (groundLayer.getTile(decoPos) != null && groundLayer.getTile(decoPos).name.Equals("sand")) continue;

                    if (rand.Next(0, 100) > biome.decoChance) continue;

                    Decoration deco = biome.chooseDecoration();

                    deco.place(layers, decoPos, biome);
                }
            }

            return layers;
        }


        public NoiseMap createBiomeNoise(int seed)
        {

            Cell gen1 = new Cell();
            gen1.Seed = seed;
            gen1.Frequency = biomeSize / 66.66;
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
        public NoiseMap createLandNoise(int seed, int size)
        {
            Perlin gen = new Perlin();
            gen.Seed = seed;
            gen.Frequency = scale / 66.66;
            gen.Persistence = 0.5;
            gen.Lacunarity = 2.25;
            gen.OctaveCount = roughness;

            ScaleBias mod = new ScaleBias();
            mod.Source0 = gen;
            mod.Bias = depth;
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
