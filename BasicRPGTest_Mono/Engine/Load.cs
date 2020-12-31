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

        public static Dictionary<Vector3, Tile> loadMap(string world) 
        {
            Dictionary < Vector3, Tile > tiles = new Dictionary<Vector3, Tile>();

            path = $"save\\{world}";
            reader = new StreamReader(path + "\\map.json");

            JObject mapJson = JObject.Parse(reader.ReadToEnd());
            JArray layers = mapJson.Value<JArray>("layers");

            foreach (JArray layerTiles in layers)
            {
                foreach (JObject tileJson in layerTiles)
                {
                    Tile template = TileManager.get(tileJson.Value<int>("id"));
                    if (template == null) continue;

                    int x = tileJson.Value<int>("x");
                    int y = tileJson.Value<int>("y");

                    tiles.Add(new Vector3(x, y, tileJson.Value<int>("z")), new Tile(template, new Vector2(x*32, y*32)));
                }
            }

            reader.Close();

            return tiles;

        }

        public static TiledMap loadMap(TiledMapTileset tileset, string world)
        {
            path = $"save\\{world}";
            reader = new StreamReader(path + "\\map.json");

            JObject mapJson = JObject.Parse(reader.ReadToEnd());
            JArray layers = mapJson.Value<JArray>("layers");

            TiledMap map = new TiledMap(mapJson.Value<string>("name"), mapJson.Value<int>("width"), mapJson.Value<int>("height"), mapJson.Value<int>("tileWidth"), mapJson.Value<int>("tileHeight"), (TiledMapTileDrawOrder)Enum.Parse(typeof(TiledMapTileDrawOrder), mapJson.Value<string>("order")), (TiledMapOrientation)Enum.Parse(typeof(TiledMapOrientation), mapJson.Value<string>("orientation")));

            map.AddTileset(tileset, 0);

            foreach (JObject layerJson in layers)
            {
                TiledMapTileLayer layer = new TiledMapTileLayer(layerJson.Value<string>("name"), map.Width, map.Height, map.TileWidth, map.TileHeight);
                JArray tilesJson = layerJson.Value<JArray>("tiles");
                foreach (JObject tileJson in tilesJson)
                {
                    layer.SetTile(tileJson.Value<ushort>("x"), tileJson.Value<ushort>("y"), tileJson.Value<uint>("id"));
                }

                map.AddLayer(layer);
            }

            reader.Close();


            return map;
        }
    }
}
