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

namespace BasicRPGTest_Mono.Engine
{
    public static class Load
    {
        public static string path { get; set; }
        public static StreamReader reader { get; set; }

        static Load()
        {
        }

        // LOAD PLAYER
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

        /*
        // LOAD MAP
        public static List<TileLayer> loadMap(string world, string map)
        {

            // Track how long this function takes to run
            Utility.CodeTimer codeTimer = new Utility.CodeTimer();
            codeTimer.startTimer();


            List<TileLayer> layers = new List<TileLayer>();

            path = $"save\\{world}\\maps";
            reader = new StreamReader($"save\\{world}\\world.json");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles();

            // Declare variables here instead of repeatedly in For loops below
            TileLayer layer;
            int layerIndex = -1;
            JArray tileArray;
            List<Tile> tiles;
            Tile template;
            int templateID;
            Tile tile;


            // Each File is a Map Tile Layer
            foreach (FileInfo file in files)
            {
                layerIndex += 1;  // Count Layer Index

                // TODO: Account for incorrect layer loading order.
                reader = new StreamReader($"{path}\\{file.Name}");
                JObject jsonLayer = JObject.Parse(reader.ReadToEnd());

                layer = new TileLayer(jsonLayer.Value<string>("layer"), layerIndex);
                tileArray = jsonLayer.Value<JArray>("tiles");
                tiles = new List<Tile>();


                // Go through each Tile in this Layer
                foreach (JObject tileJson in tileArray)
                {
                    templateID = tileJson.Value<int>("id");
                    // Grab Template Tile
                    template = TileManager.get(templateID);
                    
                    // If NO Template Tile given, skip  (it's bad)
                    if (template == null)
                    {
                        // Report Error
                        Utility.Util.myDebug(true, "Load.cs loadMap()", "Missing Tile Template (ID: " + templateID + ") on Map Tile (" + tileJson.Value<int>("x") + ", " + tileJson.Value<int>("y") + ")");
                        continue;  // Skip this Tile  (it's bad)
                    }
                    // Otherwise...

                    int x = tileJson.Value<int>("x");
                    int y = tileJson.Value<int>("y");

                    tile = new Tile(template, new Vector2(x, y));

                    layer.setTile(tile.tilePos, tile);
                    tile = null;
                }


                layers.Add(layer);
                System.Diagnostics.Debug.WriteLine("Loaded layer: " + layer.name);

                reader.Close();

                layer = null;

                GC.Collect();

            }


            // Report Load Speed
            codeTimer.endTimer();
            // Report function's speed
            Utility.Util.myDebug("Load.cs loadMap()", "Map (" + path + ") Loaded CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());


            return layers;

        }
        */

    }
}
