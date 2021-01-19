using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using RPGEngine;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;

namespace BasicRPGTest_Mono.Engine
{
    public static class Load
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

        public static string path { get; set; }
        public static StreamReader reader { get; set; }

        static Load()
        {
            mapProgress = 0;
        }

        public static Dictionary<string, Object> loadPlayer(string world)
        {
            Dictionary<string, Object> playerData = new Dictionary<string, object>();

            path = $"save\\{world}";
            reader = new StreamReader(path + "\\player.json");

            JObject playerJson = JObject.Parse(reader.ReadToEnd());
            JObject posData = playerJson.Value<JObject>("position");

            playerData.Add("position", new Vector2(posData.Value<int>("x"), posData.Value<int>("y")));

            reader.Close();

            return playerData;
        }
        public static List<TileLayer> loadMap(string world, string map)
        {

            // Track how long this function takes to run
            CodeTimer codeTimer = new CodeTimer();
            codeTimer.startTimer();


            List<TileLayer> layers = new List<TileLayer>();

            path = $"save\\{world}\\maps";
            reader = new StreamReader($"save\\{world}\\world.json");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles();
            List<Tile> tiles;

            JObject worldInfo = JObject.Parse(reader.ReadToEnd());
            int size = worldInfo.Value<int>("height");
            mapTileCount = 0;
            maxLayerTiles = size * size;
            mapTotalTiles = files.Length * maxLayerTiles;

            foreach (FileInfo file in files)
            {
                int layerTileCount = 0;
                // TODO: Account for incorrect layer loading order.
                reader = new StreamReader($"{path}\\{file.Name}");
                JObject jsonLayer = JObject.Parse(reader.ReadToEnd());

                tiles = new List<Tile>();
                TileLayer layer = new TileLayer(jsonLayer.Value<string>("layer"));
                JArray tileArray = jsonLayer.Value<JArray>("tiles");
                Tile tile;

                foreach (JObject tileJson in tileArray)
                {
                    layerTileCount++;
                    mapTileCount++;
                    Tile template = TileManager.get(tileJson.Value<int>("id"));
                    if (template == null) continue;

                    int x = tileJson.Value<int>("x");
                    int y = tileJson.Value<int>("y");
                    string biome = tileJson.Value<string>("biome");

                    tile = new Tile(template, new Vector2(x, y), BiomeManager.getByName(biome));

                    layer.setTile(tile.tilePos, tile);
                    tile = null;
                }

                mapTotalTiles -= maxLayerTiles - layerTileCount;
                mapTileCount += 0;

                layers.Add(layer);
                Save.layerTiles.Add(layer.name, tileArray);
                System.Diagnostics.Debug.WriteLine("Loaded layer: " + layer.name);

                reader.Close();

                layer = null;

                GC.Collect();

            }

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to LOAD the world.");

            System.Diagnostics.Debug.WriteLine($"Total tiles: {mapTotalTiles}");
            System.Diagnostics.Debug.WriteLine($"Total tiles: {mapTileCount}");

            return layers;

        }

    }
}
