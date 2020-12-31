using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BasicRPGTest_Mono.Engine
{
    public class Map
    {
        public string name { get; set; }
        [Obsolete("The TiledMap object is being abandoned. Replace references with appropriate replacements in the Map object.")]
        public TiledMap tiledMap { get; set; }
        public Dictionary<Vector3, Tile> tiles { get; set; }
        // TODO implement regions so we loop through regions for rendering instead of all tiles.
        public int width { get; set; }
        public int height { get; set; }
        public int widthInPixels { get; set; }
        public int heightInPixels { get; set; }

        public List<Rectangle> collidables { get; set; }

        public List<Entity> entities { get; set; }
        public List<LivingEntity> livingEntities { get; set; }

        public List<Spawn> spawns { get; set; }
        //public int totalSpawnWeights { get; set; }
        public int livingEntityCap = 50;
        public Timer spawnTimer;

        [Obsolete("The TiledMap object is being abandoned. Replace references with appropriate replacements in the Map object.")]
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
            //spawnTimer.Start();

            TiledMapTileLayer collideLayer = tiledMap.GetLayer<TiledMapTileLayer>("collide");
            foreach (TiledMapTile tile in collideLayer.Tiles)
            {
                if (tile.GlobalIdentifier != 95) continue;

                collidables.Add(new Rectangle(tile.X * tiledMap.TileWidth, tile.Y * tiledMap.TileHeight, tiledMap.TileWidth, tiledMap.TileHeight));
            }

        }
        public Map(string name, int size, Dictionary<Vector3, Tile> tiles)
        {
            this.name = name;
            this.tiles = tiles;
            this.width = size;
            this.height = size;
            this.widthInPixels = width * TileManager.dimensions;
            this.heightInPixels = height * TileManager.dimensions;
            collidables = new List<Rectangle>();

            this.entities = new List<Entity>();
            this.livingEntities = new List<LivingEntity>();
            this.spawns = new List<Spawn>();
            initSpawns();
            spawnTimer = new Timer(1000);
            spawnTimer.Elapsed += trySpawn;
            //spawnTimer.Start();

            foreach (KeyValuePair<Vector3, Tile> pair in tiles)
            {
                Vector3 pos = pair.Key;
                if (pos.Z != 1) continue;
                Tile tile = pair.Value;

                collidables.Add(new Rectangle(Convert.ToInt32(pos.X*32), Convert.ToInt32(pos.Y*32), 32, 32));
            }

            System.Diagnostics.Debug.WriteLine("Width: " + width);
            System.Diagnostics.Debug.WriteLine("Height: " + height);
            System.Diagnostics.Debug.WriteLine("PixelWidth: " + widthInPixels);
            System.Diagnostics.Debug.WriteLine("PixelHeight: " + heightInPixels);
        }

        public void initSpawns()
        {
            spawns.Add(new Spawn(EntityManager.get<LivingEntity>(3), 10));
            spawns.Add(new Spawn(EntityManager.get<LivingEntity>(1), 30));
            spawns.Add(new Spawn(EntityManager.get<LivingEntity>(2), 60));

        }

        public void Update()
        {

        }
        public void trySpawn(Object source, ElapsedEventArgs e)
        {
            if (livingEntities.Count >= livingEntityCap) return;
            Random rand = new Random();

            //if (rand.Next(0, 100) >= 25) return;

            Spawn spawn = Utility.Util.randomizeSpawn(spawns);

            System.Diagnostics.Debug.WriteLine("Successfully spawned entity " + spawn.entity.name);
            LivingEntity ent = new LivingEntity(spawn.entity, findSpawnLocation());
            livingEntities.Add(ent);

        }

        public Vector2 findSpawnLocation()
        {
            Vector2 pos = new Vector2();

            Random rand = new Random();

            int x = rand.Next(1, widthInPixels);
            int y = rand.Next(1, heightInPixels);

            pos.X = x;
            pos.Y = y;

            return pos;
        }

        public void Draw(OrthographicCamera camera, SpriteBatch batch)
        {
            foreach (KeyValuePair<Vector3, Tile> pair in tiles)
            {
                Tile tile = pair.Value;
                if (!camera.BoundingRectangle.Intersects(tile.box)) continue;
                var transformMatrix = camera.GetViewMatrix();
                batch.Begin(transformMatrix: transformMatrix);
                tile.draw(batch);
                batch.End();
            }
        }


        public float[,] createNoise()
        {
            Random seedGenerator = new Random();
            SimplexNoise.Noise.Seed = seedGenerator.Next(0, 9999999);
            float[,] values = SimplexNoise.Noise.Calc2D(128, 128, 1);

            return values;
        }
        public void testNoise()
        {

        }


        public void Clear()
        {
            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }
    }
}
