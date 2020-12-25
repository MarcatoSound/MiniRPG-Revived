using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class Generator
    {
        public static TiledMap generateOverworld(int size)
        {
            TiledMap map = new TiledMap("overworld", size, size, 32, 32, TiledMapTileDrawOrder.RightDown, TiledMapOrientation.Orthogonal);

            Random rand = new Random();
            int id;

            TiledMapTileLayer groundLayer = new TiledMapTileLayer("ground", size, size, 32, 32);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    id = rand.Next(0, 7);

                    groundLayer.SetTile(Convert.ToUInt16(x), Convert.ToUInt16(y), Convert.ToUInt32(id));
                }
            }

            TiledMapTileLayer collideLayer = new TiledMapTileLayer("collide", size, size, 32, 32);
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (rand.Next(0, 100) < 10)
                        collideLayer.SetTile(Convert.ToUInt16(x), Convert.ToUInt16(y), Convert.ToUInt32(95));
                    
                }
            }

            map.AddLayer(groundLayer);
            map.AddLayer(collideLayer);

            return map;
        }
    }
}
