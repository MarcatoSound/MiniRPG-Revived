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

        public static List<TileLayer> generateOverworldTiles(int size)
        {
            List<TileLayer> layers = new List<TileLayer>();

            Random rand = new Random();
            int id;

            List<int> groundTileChances = new List<int>();
            groundTileChances.Add(15);
            groundTileChances.Add(1);
            groundTileChances.Add(5);
            groundTileChances.Add(3);

            TileLayer groundLayer = new TileLayer("ground");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    id = Utility.Util.weightedRandom(groundTileChances) - 1;

                    groundLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(id), new Vector2(x, y)));

                }
            }
            layers.Add(groundLayer);

            TileLayer treeLayer = new TileLayer("trees");
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    if (rand.Next(0, 100) > 5) continue;
                    id = 4;

                    treeLayer.setTile(new Vector2(x, y), new Tile(TileManager.get(id), new Vector2(x, y)));

                }
            }
            layers.Add(treeLayer);

            return layers;
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
