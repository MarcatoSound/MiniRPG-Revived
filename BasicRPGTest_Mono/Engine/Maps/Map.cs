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
        //====================================================================================
        // VARIABLES
        //====================================================================================

        public string name { get; set; }
        public List<TileLayer> layers { get; set; }
        public Dictionary<Vector2, Region> regions { get; set; }
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

        private long v_drawnTileCount;

        private List<Region> v_regionsVisible = new List<Region>();
        private Dictionary<String, Dictionary<String, List<Vector2>>> v_TileCache = new Dictionary<String, Dictionary<String, List<Vector2>>>();


        //====================================================================================
        // CONSTRUCTOR
        //====================================================================================
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

            Vector2 regionTruePos = new Vector2();
            Vector2 regionPos = new Vector2();
            for (int x = 0; x < width / 8; x++)
            {
                for (int y = 0; y < height / 8; y++)
                {
                    regionTruePos.X = x * (TileManager.dimensions * 8);
                    regionTruePos.Y = y * (TileManager.dimensions * 8);
                    regionPos.X = x;
                    regionPos.Y = y;
                    Region region = new Region(regionTruePos, regionPos);
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

                    regionPos = new Vector2((int)(tile.tilePos.X / 8), (int)(tile.tilePos.Y / 8));
                    tile.region = regionPos;
                    tile.map = this;  // Tile remembers the Map it belongs to
                    tile.layer = layer;  // Tile remembers Map's Layer it belongs to
                    regions[regionPos].addTile(tile);

                    tile.update();


                    if (layer.name == "water") continue;
                    if (!tile.isCollidable) continue;

                    // Create a collidable box at the following true-map coordinate.
                    collidables.TryAdd(collidables.Count, new Rectangle(Convert.ToInt32(tile.pos.X), Convert.ToInt32(tile.pos.Y), TileManager.dimensions, TileManager.dimensions));
                }

            }
        }


        //====================================================================================
        // PROPERTIES
        //====================================================================================

        public long getTilesTotalCount()
        {
            int tCount = 0;

            foreach (TileLayer tileLayer in layers)
            {
                tCount += tileLayer.tiles.Count;
            }

            return tCount;
        }

        public long getTilesTotalCountDrawn () { return this.v_drawnTileCount; }
        public void setTilesTotalCountDrawn (long mValue) { this.v_drawnTileCount += mValue;  }


        //====================================================================================
        // FUNCTIONS
        //====================================================================================

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
        public Region getRegionByTile(Tile tile)
        {
            return regions[tile.region];
        }
        public Region getRegionByTilePosition(Vector2 tilePos)
        {
            Vector2 regionPos = new Vector2((int)(tilePos.X / 8), (int)(tilePos.Y / 8));

            return regions[regionPos];
        }
        public List<Region> getRegionsInRange(Vector2 tilePos, int radius)
        {
            List<Region> regions = new List<Region>();

            Region centerRegion = getRegionByTilePosition(tilePos);
            Vector2 topLeftRegion = new Vector2(centerRegion.regionPos.X - radius, centerRegion.regionPos.Y - radius);
            Vector2 bottomLeftRegion = new Vector2(centerRegion.regionPos.X + radius, centerRegion.regionPos.Y + radius);

            Vector2 regionPos = new Vector2();
            for (int x = (int)topLeftRegion.X; x < bottomLeftRegion.X; x++)
            {
                for (int y = (int)topLeftRegion.Y; y < bottomLeftRegion.Y; y++)
                {
                    regionPos.X = x;
                    regionPos.Y = y;
                    if (this.regions.ContainsKey(regionPos))
                        regions.Add(this.regions[regionPos]);
                }
            }

            return regions;

        }


        public void updateVisibleRegions(Camera2D camera)
        {

            foreach (Region region in regions.Values)
            {
                // If THIS Region is INSIDE Camera's view (BoundingRectangle)
                if (camera.BoundingRectangle.Intersects(region.box))
                {
                    // If List Does NOT Contain this Region
                    if (!v_regionsVisible.Contains(region))
                    {
                        // Add this Region to Collection
                        v_regionsVisible.Add(region);

                        //Utility.Util.myDebug("Region Added:  " + region.box);

                        // TODO: Add Event for on Region ADDED to regionsVisible List
                    }

                }
                // If THIS Region is OUTSIDE Camera's view (BoundingRectangle)
                else
                {
                    // If List DOES Contain this Region
                    if (v_regionsVisible.Contains(region))
                    {
                        // Add this Region to Collection
                        v_regionsVisible.Remove(region);

                        //Utility.Util.myDebug("Region Removed:  " + region.box);

                        // TODO: Add Event for on Region REMOVED from regionsVisible List
                    }
                }
            }

            // Build Tile Cache for Drawing
            buildTileCache();

        }



        public void Draw(Camera2D camera, SpriteBatch batch)
        {
            // Drawing code (Draw by Map Region, one tile at a time)

            // Clear Drawn Tiles Count (new fresh frame)
            //v_drawnTileCount = 0;

            foreach (TileLayer layer in layers)
            {
                batch.Begin(transformMatrix: Camera.camera.Transform);
                foreach (Region region in v_regionsVisible)
                {
                    region.draw(batch, layer);
                }
                batch.End();
            }

            // Report Total # of Tiles Drawn
            //Utility.Util.myDebug("Map.cs Draw()", "TILES DRAWN:  " + this.v_drawnTileCount + " of " + getTilesTotalCount());
        }

        public void DrawSpeedTest(Camera2D camera, SpriteBatch batch, int mIterationsCount)
        {
            // Start Code Timer for speed test
            Utility.CodeTimer codeTimer = new Utility.CodeTimer();
            codeTimer.startTimer();

            // If No Interation count as given, use Default of 1000
            if (mIterationsCount <= 0) { mIterationsCount = 1000; }

            for (int i = 0; i < mIterationsCount; i++)
            {
                // Draw code
                Draw(camera, batch);
            }

            // End Code Timer for speed test
            codeTimer.endTimer();
            // Report function's speed
            Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());
        }


        public void Draw_OLD(Camera2D camera, SpriteBatch batch)
        {
            // Draw code
            foreach (Region region in regions.Values)
            {
                if (!camera.BoundingRectangle.Intersects(region.box)) continue;
                batch.Begin(transformMatrix: Camera.camera.Transform);
                //region.draw(batch);
                batch.End();
            }
        }

        public void DrawSpeedTest_OLD(Camera2D camera, SpriteBatch batch, int mIterationsCount)
        {
            // Start Code Timer for speed test
            Utility.CodeTimer codeTimer = new Utility.CodeTimer();
            codeTimer.startTimer();

            // If No Interation count as given, use Default of 1000
            if (mIterationsCount <= 0) { mIterationsCount = 1000; }

            for (int i = 0; i < mIterationsCount; i++)
            {
                // Draw code
                Draw(camera, batch);
            }

            // End Code Timer for speed test
            codeTimer.endTimer();
            // Report function's speed
            Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());
        }


        public void DrawVisibleMapCache (Camera2D camera, SpriteBatch batch)
        {
            //batch.Begin(transformMatrix: Camera.camera.Transform);

            // Go through each CachedTiles Layer
            foreach (TileLayer tLayer in layers)
            {
                Dictionary<string, List<Vector2>> templateList = v_TileCache[tLayer.name];

                // Go through each Tile Template in the Layer
                foreach (string parentTileName in templateList.Keys)
                {
                    // Get Parent Tile Template
                    Tile parentTile = TileManager.getByName(parentTileName);
                    // Get Sub-Tile Locations
                    List<Vector2> list = templateList[parentTileName];
    
                parentTile.graphic.draw_Tiles(batch, list);
                }
            }

            //batch.End();

            foreach (TileLayer layer in layers)
            {
                batch.Begin(transformMatrix: Camera.camera.Transform);
                foreach (Region region in v_regionsVisible)
                {
                    region.draw(batch, layer);
                }
                batch.End();
            }

        }


        public void DrawVisibleMapTileCache_SpeedTest(Camera2D camera, SpriteBatch batch, int mIterationsCount)
        {
            // Start Code Timer for speed test
            Utility.CodeTimer codeTimer = new Utility.CodeTimer();
            codeTimer.startTimer();

            // If No Interation count as given, use Default of 1000
            if (mIterationsCount <= 0) { mIterationsCount = 1000; }

            for (int i = 0; i < mIterationsCount; i++)
            {
                // Draw code
                DrawVisibleMapCache(camera, batch);
            }

            // End Code Timer for speed test
            codeTimer.endTimer();
            // Report function's speed
            Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());
        }


        public void Clear()
        {
            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }


        private void buildTileCache()
        {
            // Clear Collection
            v_TileCache.Clear();

            Dictionary<String, List<Vector2>> tileTemplate;

            // Create top most level of CachedTiles collection (the Map Layer Names), in order of appearance in Map.layers
            // Layer Name is first Level of multi-dimensional v_TileCache collection
            foreach (TileLayer tileLayer in layers)
            {
                tileTemplate = new Dictionary<String, List<Vector2>>();

                v_TileCache.Add(tileLayer.name, tileTemplate);
            }


            // Go through Visible Regions
            foreach (Region region in v_regionsVisible)
            {
                // Go through each Tile in Region
                foreach (Tile tile in region.tiles)
                {
                    String tileParentName = tile.parent.name;
                    string layerName = tile.layer.name;

                    // Get Tile Template Dictionary matching layerName (Layer Key)
                    tileTemplate = v_TileCache[layerName];

                    // If tileTemplate Dictionary does NOT have this Tile Template Name (key) already
                    if (!tileTemplate.ContainsKey(tileParentName))
                        {
                        // Create List of Vector positions. List will contain Positions of ALL matching Tiles.
                        List<Vector2> list = new List<Vector2>();
                        // Add Position to TileCache List
                        list.Add(tile.drawPos);

                        // Add Tile template with this Tile's Position to Collection
                        tileTemplate.Add(tileParentName, list);

                    } else
                    {
                        // Get Parent Tile's sub-tile position List
                        List<Vector2> list = tileTemplate[tileParentName];
                        // Add Position to TileCache List
                        list.Add(tile.drawPos);
                    }

                }
            }

        }


    }
}
