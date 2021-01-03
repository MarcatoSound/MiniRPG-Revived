using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using RPGEngine;
using BasicRPGTest_Mono.Engine.Maps;

namespace BasicRPGTest_Mono.Engine
{
    public static class Save
    {
        public static string path { get; set; }
        public static TextWriter writer { get; set; }
        static Save()
        {
        }

        public static void save(Player player, string world)
        {
            path = $"save\\{world}";

            if (!Directory.Exists(path))
            {
                DirectoryInfo dInfo = Directory.CreateDirectory(path);
            }

            writer = new StreamWriter(path + "\\player.json", false);
            JObject json = new JObject();

            // Save the map's general info
            System.Diagnostics.Debug.WriteLine("## Saving player info!");

            JObject jsonPos = new JObject();
            jsonPos.Add("x", player.Position.X);
            jsonPos.Add("y", player.Position.Y);

            json.Add("position", jsonPos);

            try
            {
                writer.Write(json.ToString(Newtonsoft.Json.Formatting.Indented));
                System.Diagnostics.Debug.WriteLine("Successfully saved player data!");
            }
            finally
            {
                writer.Close();
            }

        }

        public static void save(Map map, string world)
        {
            path = $"save\\{world}";

            if (!Directory.Exists(path))
            {
                DirectoryInfo dInfo = Directory.CreateDirectory(path);
            }

            writer = new StreamWriter(path + "\\map.json", false);
            JObject json = new JObject();

            // Save the map's general info
            System.Diagnostics.Debug.WriteLine("## Saving map info!");
            JToken name = new JValue(map.name);
            JToken height = new JValue(map.height);
            JToken width = new JValue(map.width);

            json.Add("name", name);
            json.Add("height", height);
            json.Add("width", width);

            JArray layers = new JArray();
            JObject jsonLayer;
            JArray layerTiles;

            JObject tileData;
            foreach (TileLayer layer in map.layers)
            {
                jsonLayer = new JObject();
                layerTiles = new JArray();

                JToken layerName = new JValue(layer.name);
                jsonLayer.Add("layer", layerName);

                foreach (Tile tile in layer.tiles.Values)
                {

                    tileData = new JObject();

                    // Save the tile data and add it to the layer json
                    JToken tileId = new JValue(tile.id);
                    JToken tileX = new JValue(tile.tilePos.X);
                    JToken tileY = new JValue(tile.tilePos.Y);

                    tileData.Add("id", tileId);
                    tileData.Add("x", tileX);
                    tileData.Add("y", tileY);

                    layerTiles.Add(tileData);
                }

                jsonLayer.Add("tiles", layerTiles);
                layers.Add(jsonLayer);
            }

            json.Add("layers", layers);

            try
            {
                writer.Write(json.ToString(Newtonsoft.Json.Formatting.Indented));
                System.Diagnostics.Debug.WriteLine("Successfully saved map data!");
            }
            finally
            {
                writer.Close();
            }

        }

    }
}
