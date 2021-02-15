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
using YamlDotNet.RepresentationModel;
using System.Threading;

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

        public static YamlSection loadPlayer(string world)
        {
            string file = $"save\\{world}\\player.yml";

            Console.WriteLine($"// Reading player file...");
            var reader = new StreamReader(file);
            var input = new StringReader(reader.ReadToEnd());

            YamlStream yaml = new YamlStream();
            yaml.Load(input);

            YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            return new YamlSection(mapping);
        }
        /*public static Dictionary<string, Object> loadPlayer(string world)
        {
            Dictionary<string, Object> playerData = new Dictionary<string, object>();

            path = $"save\\{world}";
            reader = new StreamReader(path + "\\player.json");

            JObject playerJson = JObject.Parse(reader.ReadToEnd());
            JObject posData = playerJson.Value<JObject>("position");

            playerData.Add("position", new Vector2(posData.Value<int>("x"), posData.Value<int>("y")));

            reader.Close();

            return playerData;
        }*/
        public static List<Map> loadMaps(string world)
        {
            List<Map> maps = new List<Map>();

            path = $"save\\{world}\\maps";

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] folders = dirInfo.GetDirectories();

            StreamReader reader = new StreamReader($"save\\{world}\\player.yml");
            var input = new StringReader(reader.ReadToEnd());

            YamlStream yaml = new YamlStream();
            yaml.Load(input);

            YamlSection player = new YamlSection((YamlMappingNode)yaml.Documents[0].RootNode);
            Vector2 playerPos = new Vector2((float)player.getDouble("position.x"), (float)player.getDouble("position.y"));
            string playerMap = player.getString("map");
            reader.Close();

            foreach (DirectoryInfo mapFolder in folders)
            {
                string mapPath = $"{path}\\{mapFolder.Name}";

                
                reader = new StreamReader($"{mapPath}\\map.yml");
                input = new StringReader(reader.ReadToEnd());
                yaml.Load(input);

                YamlSection general = new YamlSection((YamlMappingNode)yaml.Documents[0].RootNode);

                Map map = new Map(general, world);
                reader.Close();
                if (map.name == "") continue;

                // Load the local tile data
                if (playerMap.Equals(map.name))
                {
                    MapManager.activeMap = map;

                    Vector2 tilePos = Util.getTilePosition(playerPos);
                    List<Region> regions = map.getRegionsInRange(tilePos, 2);

                    foreach (Region region in regions)
                    {
                        loadRegion(world, map, region);
                    }
                }

                // Load the living entities
                reader = new StreamReader($"{mapPath}\\entities.yml");
                input = new StringReader(reader.ReadToEnd());
                yaml.Load(input);

                YamlSequenceNode entities = (YamlSequenceNode)yaml.Documents[0].RootNode;
                foreach (YamlMappingNode entityData in entities)
                {
                    YamlSection entity = new YamlSection(entityData);
                    map.loadEntity(entity);
                }
                reader.Close();

                // Load the item entities
                reader = new StreamReader($"{mapPath}\\item_drops.yml");
                input = new StringReader(reader.ReadToEnd());
                yaml.Load(input);

                YamlSequenceNode itemEntities = (YamlSequenceNode)yaml.Documents[0].RootNode;
                foreach (YamlMappingNode itemData in itemEntities)
                {
                    YamlSection item = new YamlSection(itemData);
                    map.loadItemEntity(item);
                }
                reader.Close();

                // Update the tile edges
                foreach (TileLayer layer in map.layers)
                {
                    foreach (KeyValuePair<Vector2, Tile> pair in layer.tiles)
                    {
                        Vector2 pos = pair.Key;
                        Tile tile = pair.Value;

                        tile.update();
                    }
                }

                MapManager.add(map);
                map.buildTileTemplateCache();
                map.buildVisibleTileCache();
            }

            return maps;

        }
        public static void loadRegions(string world, Map map, List<Region> regions)
        {
            path = $"save\\{world}\\maps";
            string mapPath = $"{path}\\{map.name}";

            List<Region> loadedRegions = new List<Region>();

            foreach (Region region in regions)
            {
                if (region.tiles.Count != 0) continue;
                loadedRegions.Add(region);
                string regionFile = $"reg_{(int)region.regionPos.X}-{(int)region.regionPos.Y}";

                reader = new StreamReader($"{mapPath}\\regions\\{regionFile}.yml");
                var input = new StringReader(reader.ReadToEnd());
                YamlStream yamlRegion = new YamlStream();
                yamlRegion.Load(input);
                YamlMappingNode regionNode = (YamlMappingNode)yamlRegion.Documents[0].RootNode;
                map.loadRegion(new YamlSection(regionNode));
                reader.Close();
            }

            foreach (Region region in loadedRegions)
            {
                foreach (Tile tile in region.tiles)
                {
                    tile.update();
                }
            }
        }
        public static void loadRegion(string world, Map map, Region region)
        {
            if (region.tiles.Count != 0) return;

            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                path = $"save\\{world}\\maps";
                string mapPath = $"{path}\\{map.name}";

                string regionFile = $"reg_{(int)region.regionPos.X}-{(int)region.regionPos.Y}";

                StreamReader reader = new StreamReader($"{mapPath}\\regions\\{regionFile}.yml");
                var input = new StringReader(reader.ReadToEnd());
                YamlStream yamlRegion = new YamlStream();
                yamlRegion.Load(input);
                YamlMappingNode regionNode = (YamlMappingNode)yamlRegion.Documents[0].RootNode;
                map.loadRegion(new YamlSection(regionNode));

                reader.Close();
            });
            thread.Start();
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

                    // TODO: Tile biomes are NOT SAVED!! Fix this!
                    tile = new Tile(template, new Vector2(x, y), BiomeManager.getByName(biome));

                    layer.setTile(tile.tilePos, tile);
                    tile = null;
                }

                mapTotalTiles -= maxLayerTiles - layerTileCount;
                mapTileCount += 0;

                layers.Add(layer);
                Save.layerTiles.Add(layer.name, tileArray);
                Console.WriteLine("Loaded layer: " + layer.name);

                reader.Close();

                layer = null;

                GC.Collect();

            }

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to LOAD the world.");

            //Console.WriteLine($"Total tiles: {mapTotalTiles}");
            //Console.WriteLine($"Total tiles: {mapTileCount}");

            return layers;

        }

    }
}
