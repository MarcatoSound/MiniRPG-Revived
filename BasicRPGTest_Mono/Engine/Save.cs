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
        public static Dictionary<string, Map> oldMapStates { get; set; }
        public static Dictionary<string, JArray> layerTiles { get; set; }

        public static string path { get; set; }
        public static TextWriter writer { get; set; }
        static Save()
        {
            oldMapStates = new Dictionary<string, Map>();
            layerTiles = new Dictionary<string, JArray>();
        }

        public static void save(Player player, string world)
        {
            CodeTimer codeTimer = new CodeTimer();
            codeTimer.startTimer();

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

            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to SAVE the player.");

            GC.Collect();

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to SAVE the player and GC.");

        }

        public static void save(Map map, string world)
        {
            if (!oldMapStates.ContainsKey(map.name))
            {
                // This map doesn't have a previous state, therefore it is probably brand new. Don't bother saving this time.
                //   By default, maps are added to the "oldMapStates" list on load, but just in case it isn't...
                oldMapStates.Add(map.name, map);
                return;
            }

            Map oldMap = oldMapStates[map.name];
            // Later, add a list to the map object tracking changes.

            CodeTimer codeTimer = new CodeTimer();
            codeTimer.startTimer();
            path = $"save\\{world}\\maps";

            if (!Directory.Exists(path))
            {
                DirectoryInfo dInfo = Directory.CreateDirectory(path);
            }

            JObject worldJson = new JObject();

            CodeTimer subTimer = new CodeTimer();
            subTimer.startTimer();
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

            subTimer.endTimer();
            Util.myDebug($"Took {subTimer.getTotalTimeInMilliseconds()}ms to SAVE general world info.");

            // Save the map's layer data
            JObject jsonLayer;
            JArray layerTiles;

            JObject tileData;
            int layerNumber = 0;
            foreach (TileLayer layer in oldMap.layers)
            {
                CodeTimer subSubTimer = new CodeTimer();

                subSubTimer.startTimer();
                TileLayer newLayer = map.getLayer(layer.name);
                if (newLayer == null) continue;

                subTimer.clearTimer();
                subTimer.startTimer();
                writer = new StreamWriter($"{path}\\{map.name}_{layerNumber}.json", false);

                jsonLayer = new JObject();
                // If there isn't a JArray for this layer already, create it and add it.
                bool isNewArray = false;
                if (!Save.layerTiles.ContainsKey(layer.name))
                {
                    Save.layerTiles.Add(layer.name, new JArray());
                    isNewArray = true;
                }
                layerTiles = Save.layerTiles[layer.name];

                JToken layerName = new JValue(layer.name);
                jsonLayer.Add("layer", layerName);

                int entry = 0;

                subSubTimer.endTimer();
                Util.myDebug($"Preliminary set up took {subSubTimer.getTotalTimeInMilliseconds()}ms on layer {layer.name}.");
                // TODO: WARNING!!! This will NOT save new tiles being placed!!
                subSubTimer.clearTimer();
                subSubTimer.startTimer();
                foreach (Tile tile in layer.tiles.Values)
                {
                    if (!isNewArray)
                    {
                        // Perform the check to see if this tile is the same as its old state.
                        Tile newTile;
                        if (newLayer.getTile(tile.tilePos) == null) continue;
                        newTile = newLayer.getTile(tile.tilePos);
                        if (tile.Equals(newTile)) continue;
                    }


                    tileData = new JObject();

                    // Save the tile data and add it to the layer json
                    JToken tileId = new JValue(tile.id);
                    JToken tileX = new JValue(tile.tilePos.X);
                    JToken tileY = new JValue(tile.tilePos.Y);

                    tileData.Add("id", tileId);
                    tileData.Add("x", tileX);
                    tileData.Add("y", tileY);

                    if (isNewArray)
                        layerTiles.Add(tileData);
                    else
                    {
                        layerTiles[entry].Replace(tileData);
                    }

                    tileData = null;
                    entry++;
                }
                subSubTimer.endTimer();
                Util.myDebug($"Looping through tiles took {subSubTimer.getTotalTimeInMilliseconds()}ms on layer {layer.name}.");

                subSubTimer.clearTimer();
                subSubTimer.startTimer();
                jsonLayer.Add("tiles", layerTiles);
                subSubTimer.endTimer();
                Util.myDebug($"Adding tiles to JSON layer took {subSubTimer.getTotalTimeInMilliseconds()}ms on layer {layer.name}.");

                subSubTimer.clearTimer();
                subSubTimer.startTimer();
                try
                {
                    writer.Write(jsonLayer.ToString(Newtonsoft.Json.Formatting.Indented));
                }
                finally
                {
                    writer.Close();
                }
                subSubTimer.endTimer();
                Util.myDebug($"Saving tiles to file took {subSubTimer.getTotalTimeInMilliseconds()}ms on layer {layer.name}.");

                layerTiles = null;
                jsonLayer = null;

                GC.Collect();

                layerNumber++;
                subTimer.endTimer();
                Util.myDebug($"Took {subTimer.getTotalTimeInMilliseconds()}ms to SAVE layer {layer.name}.");
            }

            oldMapStates.Remove(map.name);
            oldMapStates.Add(map.name, new Map(map));

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to SAVE the WHOLE world.");

        }

    }
}
