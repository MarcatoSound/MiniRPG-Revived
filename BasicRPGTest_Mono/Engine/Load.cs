using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BasicRPGTest_Mono.Engine
{
    public static class Load
    {
        public static string path { get; set; }
        public static StreamReader reader { get; set; }

        static Load()
        {
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
