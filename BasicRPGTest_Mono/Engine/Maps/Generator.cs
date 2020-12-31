using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class Generator
    {

        public static Dictionary<Vector3, Tile> generateOverworldTiles(int size)
        {
            Dictionary<Vector3, Tile> tiles = new Dictionary<Vector3, Tile>();

            Random rand = new Random();
            int id;

            List<int> groundTileChances = new List<int>();
            groundTileChances.Add(15);
            groundTileChances.Add(1);
            groundTileChances.Add(5);
            groundTileChances.Add(3);

            for (int z = 0; z < 2; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        switch (z)
                        {
                            case 0:
                                
                                id = Utility.Util.weightedRandom(groundTileChances)-1;

                                tiles.Add(new Vector3(x, y, z), new Tile(TileManager.get(id), new Vector2(x*32, y*32)));
                                break;
                            case 1:
                                if (rand.Next(0, 100) > 5) continue;
                                id = 4;

                                tiles.Add(new Vector3(x, y, z), new Tile(TileManager.get(id), new Vector2(x*32, y*32)));
                                break;
                        }
                    }
                }
            }

            return tiles;
        }
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
