using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using RPGEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine
{
    public class Map
    {
        public string name { get; set; }
        public List<TileLayer> layers { get; set; }
        public Dictionary<Vector2, Region> regions { get; set; }
        // TODO implement regions so we loop through regions for rendering instead of all tiles.
        public int width { get; set; }
        public int height { get; set; }
        public int widthInPixels { get; set; }
        public int heightInPixels { get; set; }

        public ConcurrentDictionary<int, Rectangle> collidables { get; set; }

        public ConcurrentDictionary<int, Entity> entities { get; set; }
        public ConcurrentDictionary<int, LivingEntity> livingEntities { get; set; }

        public ConcurrentDictionary<int, Spawn> spawns { get; set; }
        //public int totalSpawnWeights { get; set; }
        public int livingEntityCap = 50;
        public Timer spawnTimer;
        public Map(string name, int size, List<TileLayer> layers) 
        {
            this.name = name;
            this.layers = layers;
            this.width = size;
            this.height = size;
            this.widthInPixels = width * TileManager.dimensions;
            this.heightInPixels = height * TileManager.dimensions;
            regions = new Dictionary<Vector2, Region>();
            collidables = new ConcurrentDictionary<int, Rectangle>();

            this.entities = new ConcurrentDictionary<int, Entity>();
            this.livingEntities = new ConcurrentDictionary<int, LivingEntity>();
            this.spawns = new ConcurrentDictionary<int, Spawn>();
            initSpawns();
            spawnTimer = new Timer(1000);
            spawnTimer.Elapsed += trySpawn;
            spawnTimer.Start();

            for (int x = 0; x < width / 8; x++)
            {
                for (int y = 0; y < height / 8; y++)
                {
                    Region region = new Region(new Vector2(x * (TileManager.dimensions * 8), y * (TileManager.dimensions * 8)));
                    regions.Add(new Vector2(x, y), region);
                }
            }


            foreach (TileLayer layer in layers)
            {
                Dictionary<Vector2, Tile> tiles = layer.tiles;
                foreach (KeyValuePair<Vector2, Tile> pair in tiles)
                {
                    Vector2 pos = pair.Key;
                    Tile tile = pair.Value;

                    Vector2 regionPos = new Vector2((int)(tile.tilePos.X / 8), (int)(tile.tilePos.Y / 8));
                    tile.region = regionPos;
                    regions[regionPos].addTile(tile);
                    tile.update();


                    if (layer.name == "water") continue;
                    if (!tile.isCollidable) continue;

                    // Create a collidable box at the following true-map coordinate.
                    collidables.TryAdd(collidables.Count, new Rectangle(Convert.ToInt32(tile.pos.X), Convert.ToInt32(tile.pos.Y), TileManager.dimensions, TileManager.dimensions));
                }

            }


        }

        public void initSpawns()
        {
            spawns.TryAdd(spawns.Keys.Count, new Spawn(EntityManager.get<LivingEntity>(0), 2));
            spawns.TryAdd(spawns.Keys.Count, new Spawn(EntityManager.get<LivingEntity>(1), 3));
            spawns.TryAdd(spawns.Keys.Count, new Spawn(EntityManager.get<LivingEntity>(2), 1));
        }

        public void Update()
        {

        }
        public void trySpawn(Object source, ElapsedEventArgs e)
        {
            if (livingEntities.Count >= livingEntityCap) return;
            Random rand = new Random();

            if (rand.Next(0, 100) >= 25) return;

            Spawn spawn = Utility.Util.randomizeSpawn(spawns);

            //System.Diagnostics.Debug.WriteLine("Successfully spawned entity " + spawn.entity.name);

            Vector2 target = findSpawnLocation(spawn.entity);

            LivingEntity ent = new LivingEntity(spawn.entity, target, livingEntities.Count);
            livingEntities.TryAdd(livingEntities.Keys.Count, ent);

        }

        public Vector2 findSpawnLocation(LivingEntity ent)
        {
            Vector2 pos = new Vector2();

            Random rand = new Random();

            int x = rand.Next(32, widthInPixels - 32);
            int y = rand.Next(32, heightInPixels - 32);

            pos.X = x;
            pos.Y = y;

            Rectangle target = new Rectangle(x, y, ent.boundingBox.Width, ent.boundingBox.Height);
            if (!isLocationSafe(target))
            {
                return findSpawnLocation(ent);
            }

            return pos;
        }
        public bool isLocationSafe(Rectangle location)
        {
            foreach (Rectangle collidable in collidables.Values)
            {
                collidable.Inflate(5, 5);
                if (location.Intersects(collidable)) return false;
            }
            return true;
        }

        public void Draw(Camera2D camera, SpriteBatch batch)
        {
            batch.Begin(transformMatrix: Camera.camera.Transform);
            foreach (Region region in regions.Values)
            {
                if (!camera.BoundingRectangle.Intersects(region.box)) continue;
                foreach (TileLayer layer in layers)
                {
                    region.draw(batch, layer);
                }
            }
            batch.End();
        }


        public void Clear()
        {
            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }
    }
}
