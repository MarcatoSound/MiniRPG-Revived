using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework;
using RPGEngine;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;

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

            GC.Collect();

        }

        public static void save(Map map, string world)
        {
            CodeTimer codeTimer = new CodeTimer();
            codeTimer.startTimer();
            path = $"save\\{world}\\maps";

            if (!Directory.Exists(path))
            {
                DirectoryInfo dInfo = Directory.CreateDirectory(path);
            }

            JObject worldJson = new JObject();

            // Save the map's general info
            System.Diagnostics.Debug.WriteLine("## Saving map info!");
            JToken name = new JValue(map.name);
            JToken height = new JValue(map.height);
            JToken width = new JValue(map.width);

            worldJson.Add("name", name);
            worldJson.Add("height", height);
            worldJson.Add("width", width);

            writer = new StreamWriter($"save\\{world}\\world.json", false);

            try
            {
                writer.Write(worldJson.ToString(Newtonsoft.Json.Formatting.Indented));
            }
            finally
            {
                writer.Close();
            }

            // Save the map's layer data
            JObject jsonLayer;
            JArray layerTiles;

            JObject tileData;
            int layerNumber = 0;
            foreach (TileLayer layer in map.layers)
            {
                writer = new StreamWriter($"{path}\\{map.name}_{layerNumber}.json", false);
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

                    tileData = null;
                }

                jsonLayer.Add("tiles", layerTiles);

                try
                {
                    writer.Write(jsonLayer.ToString(Newtonsoft.Json.Formatting.Indented));
                }
                finally
                {
                    writer.Close();
                }

                layerTiles = null;
                jsonLayer = null;

                GC.Collect();

                layerNumber++;
            }

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to SAVE the world.");

        }

    }
}
