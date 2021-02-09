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
using BasicRPGTest_Mono.Engine.Utility;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine
{
    public static class MapGeneration
    {
        private static int _mapTileCount;

        public static int mapTotalTiles { get; private set; }
        public static int mapTileCount
        {
            get { return _mapTileCount; }
            private set
            {
                mapProgress = (double)value / (double)mapTotalTiles;
                _mapTileCount = value;
            }
        }
        private static int maxLayerTiles { get; set; }
        public static double mapProgress { get; private set; }

        public static List<TileLayer> generateLayers(YamlSection config, int size)
        {

            double waterLevel = (config.getDouble("water_frequency", 0.5) * 2) - 1;
            double sandLevel = waterLevel + 0.07;
            double stoneLevel = 2 - (config.getDouble("stone_frequency", 0.25) * 2);

            mapTileCount = 0;
            maxLayerTiles = size * size;
            mapTotalTiles = maxLayerTiles;


            List<TileLayer> layers = new List<TileLayer>();

            Random rand = new Random();
            Random seedGenerator = new Random();
            int seed = seedGenerator.Next(0, 9999999);
            System.Diagnostics.Debug.WriteLine($"Seed: {seed}");

            NoiseMap biomeNoise = createBiomeNoise(config.getDouble("scale", 1), seed);
            Biome biome;

            YamlNode noiseConfig = config.get("advanced");
            NoiseMap noise;
            if (noiseConfig.NodeType == YamlNodeType.Mapping)
                noise = createLandNoise(new YamlSection("advanced", (YamlMappingNode)noiseConfig), seed, size);
            else
                noise = createLandNoise(seed, size);


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
                    mapTileCount++;
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
        public static List<TileLayer> generateOverworld(int size)
        {
            mapTileCount = 0;
            maxLayerTiles = size * size;
            mapTotalTiles = maxLayerTiles; 


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
                    mapTileCount++;
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
                    if (noise[x, y] < 0)
                        waterLayer.setTile(new Vector2(x, y), new Tile(TileManager.getByName("water"), new Vector2(x, y), biome));

                    // Ground layer
                    if (noise[x, y] >= 0 && noise[x, y] < 0.07)
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.coastTile, new Vector2(x, y), biome));
                    else if (noise[x, y] >= 0 && noise[x, y] <= 0.5)
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.groundTile, new Vector2(x, y), biome));
                    else if (noise[x, y] > 0.5)
                        groundLayer.setTile(new Vector2(x, y), new Tile(biome.undergroundTile, new Vector2(x, y), biome));

                    // Stone layer
                    if (noise[x, y] > 0.5)
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

            /*
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
                }
            }

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

                }
            }

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

                    //decoration.setTile(new Vector2(x, y), new Tile(TileManager.getByName("tree"), new Vector2(x, y), biome));

                }
            }*/

            return layers;
        }


        /// <summary>
        /// Generates cell noise for determining the distribution of tile biomes.
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
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
        public static NoiseMap createBiomeNoise(double scale, int seed)
        {

            Cell gen1 = new Cell();
            gen1.Seed = seed;
            gen1.Frequency = scale / 66.66;
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
        public static NoiseMap createLandNoise(YamlSection config, int seed, int size)
        {
            Perlin gen = new Perlin();
            gen.Seed = seed;
            gen.Frequency = config.getDouble("scale", 1) / 66.66;
            gen.Persistence = 0.5;
            gen.Lacunarity = 2.25;
            gen.OctaveCount = config.getInt("roughness", 4);

            ScaleBias mod = new ScaleBias();
            mod.Source0 = gen;
            mod.Bias = config.getDouble("depth", 0.2);
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
