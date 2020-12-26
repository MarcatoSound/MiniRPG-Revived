using BasicRPGTest_Mono.Engine.Entities;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine
{
    public class Map
    {
        public string name { get; set; }
        public TiledMap tiledMap { get; set; }

        public List<Rectangle> collidables { get; set; }

        public List<Entity> entities { get; set; }
        public List<LivingEntity> livingEntities { get; set; }
        public List<Spawn> spawns { get; set; }
        public int totalSpawnWeights { get; set; }
        public int livingEntityCap = 50;
        public Timer spawnTimer;

        public Map(TiledMap tiledMap)
        {
            this.tiledMap = tiledMap;
            name = tiledMap.Name;
            collidables = new List<Rectangle>();

            this.entities = new List<Entity>();
            this.livingEntities = new List<LivingEntity>();
            this.spawns = new List<Spawn>();
            initSpawns();
            spawnTimer = new Timer(1000);
            spawnTimer.Elapsed += trySpawn;
            spawnTimer.Start();

            TiledMapTileLayer collideLayer = tiledMap.GetLayer<TiledMapTileLayer>("collide");
            foreach (TiledMapTile tile in collideLayer.Tiles)
            {
                if (tile.GlobalIdentifier != 95) continue;

                collidables.Add(new Rectangle(tile.X * tiledMap.TileWidth, tile.Y * tiledMap.TileHeight, tiledMap.TileWidth, tiledMap.TileHeight));
            }

        }

        public void initSpawns()
        {
            spawns.Add(new Spawn(EntityManager.get<LivingEntity>(1), 50));
            spawns.Add(new Spawn(EntityManager.get<LivingEntity>(2), 100));
            spawns.Add(new Spawn(EntityManager.get<LivingEntity>(3), 10));

            foreach (Spawn spawn in spawns)
            {
                totalSpawnWeights += spawn.weight;
            }
        }

        public void Update()
        {

        }
        public void trySpawn(Object source, ElapsedEventArgs e)
        {
            if (livingEntities.Count >= livingEntityCap) return;
            Random rand = new Random();

            if (rand.Next(0, 100) >= 25) return;

            int result = rand.Next(0, totalSpawnWeights);

            foreach (Spawn spawn in spawns)
            {
                if (result < spawn.weight)
                {
                    System.Diagnostics.Debug.WriteLine("Successfully spawned entity " + spawn.entity.id);
                    LivingEntity ent = new LivingEntity(spawn.entity, findSpawnLocation());
                    livingEntities.Add(ent);
                    break;
                }

                result -= spawn.weight;
            }

        }

        public Vector2 findSpawnLocation()
        {
            Vector2 pos = new Vector2();

            Random rand = new Random();

            int x = rand.Next(1, tiledMap.WidthInPixels);
            int y = rand.Next(1, tiledMap.HeightInPixels);

            pos.X = x;
            pos.Y = y;

            return pos;
        }


        public void Clear()
        {
            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }
    }
}
