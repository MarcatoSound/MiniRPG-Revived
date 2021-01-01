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
            System.Diagnostics.Debug.WriteLine("## Saving general info!");
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

        public static void save(TiledMap map, string world)
        {
            path = $"save\\{world}";

            if (!Directory.Exists(path))
            {
                DirectoryInfo dInfo = Directory.CreateDirectory(path);
            }

            writer = new StreamWriter(path + "\\map.json", false);
            JObject json = new JObject();

            // Save the map's general info
            System.Diagnostics.Debug.WriteLine("## Saving general info!");
            JToken name = new JValue(map.Name);
            JToken height = new JValue(map.Height);
            JToken width = new JValue(map.Width);
            JToken tileHeight = new JValue(map.TileHeight);
            JToken tileWidth = new JValue(map.TileWidth);
            JToken orientation = new JValue(map.Orientation.ToString());
            JToken order = new JValue(map.RenderOrder.ToString());
            JToken bgColor = new JValue(map.BackgroundColor);

            json.Add("name", name);
            json.Add("height", height);
            json.Add("width", width);

            //json.Add("tileHeight", tileHeight);
            //json.Add("tileWidth", tileWidth);
            //json.Add("orientation", orientation);
            //json.Add("order", order);
            //json.Add("bgColor", bgColor);


            // Now the fun part... saving the child objects...
            // Lets start with the tilesets
            /*JArray tilesets = new JArray();
            JObject tilesetData;
            JArray tiles;
            foreach (TiledMapTileset tileset in map.Tilesets)
            {
                tilesetData = new JObject();

                // Save the general tileset data
                JToken tilesetName = new JValue(tileset.Name);
                JToken rows = new JValue(tileset.Rows);
                JToken tileCount = new JValue(tileset.TileCount);
                JToken tilesetTileHeight = new JValue(tileset.TileHeight);
                JToken tilesetTileWidth = new JValue(tileset.TileWidth);
                JToken textureName = new JValue(tileset.Texture.Name);

                tilesetData.Add("name", tilesetName);
                tilesetData.Add("rows", rows);
                tilesetData.Add("tileCount", tileCount);
                tilesetData.Add("tileHeight", tilesetTileHeight);
                tilesetData.Add("tileWidth", tilesetTileWidth);
                tilesetData.Add("textureName", textureName);

                tiles = new JArray();
                JObject tileData;
                foreach (TiledMapTilesetTile tile in tileset.Tiles)
                {
                    tileData = new JObject();
                    // Save the tile data and add it to the tileset json
                    JToken tileId = new JValue(tile.LocalTileIdentifier);


                    tiles.Add(tileData);
                }
                tilesetData.Add("tiles", tiles);

                tilesets.Add(tilesetData);
            }*/

            // Next the tile layers
            JArray layers = new JArray();
            JObject layerData;
            JArray layerTiles;
            JArray tiles;
            System.Diagnostics.Debug.WriteLine("## Saving map tile layers!");
            foreach (TiledMapTileLayer layer in map.TileLayers)
            {
                layerData = new JObject();

                // Save the general layer data
                JToken layerName = new JValue(layer.Name);
                JToken isVisible = new JValue(layer.IsVisible);
                JToken opacity = new JValue(layer.Opacity);
                // Don't forget that, when loading the layer, we need to apply the tile size and layer size

                layerData.Add("name", layerName);
                layerData.Add("isVisible", isVisible);
                layerData.Add("opacity", opacity);

                // Now lets loop through the tiles on this layer
                tiles = new JArray();
                JObject tileData;
                foreach (TiledMapTile tile in layer.Tiles)
                {
                    tileData = new JObject();

                    // Save the tile data and add it to the layer json
                    JToken tileId = new JValue(tile.GlobalIdentifier);
                    JToken tileX = new JValue(tile.X);
                    JToken tileY = new JValue(tile.Y);

                    tileData.Add("id", tileId);
                    tileData.Add("x", tileX);
                    tileData.Add("y", tileY);

                    tiles.Add(tileData);

                }

                layerData.Add("tiles", tiles);
                layers.Add(layerData);
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
