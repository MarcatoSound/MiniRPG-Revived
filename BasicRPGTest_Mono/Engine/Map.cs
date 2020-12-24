using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Map
    {
        public string name { get; set; }
        public TiledMap tiledMap { get; set; }

        public List<Rectangle> collidables { get; set; }

        public Map(TiledMap tiledMap)
        {
            this.tiledMap = tiledMap;
            collidables = new List<Rectangle>();
            name = tiledMap.Name;

            TiledMapTileLayer collideLayer = tiledMap.GetLayer<TiledMapTileLayer>("collide");
            foreach (TiledMapTile tile in collideLayer.Tiles)
            {
                if (tile.GlobalIdentifier != 95) continue;

                collidables.Add(new Rectangle(tile.X * tiledMap.TileWidth, tile.Y * tiledMap.TileHeight, tiledMap.TileWidth, tiledMap.TileHeight));
            }

        }
    }
}
