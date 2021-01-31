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

        public static List<TileLayer> generateOverworldTiles(Map map)
        {
            int mapWidth = map.width;
            int mapHeight = map.height;
            
            List<TileLayer> layers = new List<TileLayer>();

            Random rand = new Random();
            int id;

            List<int> groundTileChances = new List<int>();
            groundTileChances.Add(15);
            groundTileChances.Add(1);
            groundTileChances.Add(5);
            groundTileChances.Add(3);

            NoiseMap noise = createLandNoise();


            // Add Water Layer
            TileLayer waterLayer = new TileLayer(map, "water", 0);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (noise[x, y] < 0)
                    {
                        waterLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(Convert.ToInt32(5)), new Vector2(x, y)));
                        continue;
                    }
                }
            }
            layers.Add(waterLayer);


            // Add Ground Layer
            TileLayer groundLayer = new TileLayer(map, "ground", 1);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
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


            // Add Stone Layer
            TileLayer stoneLayer = new TileLayer(map, "stone", 2);
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (noise[x, y] > 0.5)
                    {
                        stoneLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(Convert.ToInt32(2)), new Vector2(x, y)));
                        continue;
                    }
                }
            }
            layers.Add(stoneLayer);


            // Add Decorations Layer
            TileLayer decoration = new TileLayer(map, "decorations", 3);
            Vector2 treePos = new Vector2();
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
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


            // Add Edge Layer
            // TODO: ("Engine\Maps\Generator.cs") Generate Tile Edges (Layer) based on previous layers.




            return layers;
        }


        public static NoiseMap createLandNoise()
        {
            Random seedGenerator = new Random();
            int seed = seedGenerator.Next(0, 9999999);
            System.Diagnostics.Debug.WriteLine($"Seed: {seed}");
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
            builder.SetBounds(0, 512, 0, 512);
            NoiseMap map = new NoiseMap(512, 512);
            builder.DestNoiseMap = map;
            builder.SetDestSize(512, 512);
            builder.Build();

            return map;
        }
        public static float[,] createNoise()
        {
            Random seedGenerator = new Random();
            SimplexNoise.Noise.Seed = seedGenerator.Next(0, 9999999);
            float[,] values = SimplexNoise.Noise.Calc2D(128, 128, 0.025f);

            return values;
        }
        public static void testNoise()
        {

        }

    }
}
