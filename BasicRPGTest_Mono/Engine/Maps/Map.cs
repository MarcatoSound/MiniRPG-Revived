using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Maps.Generation;
using BasicRPGTest_Mono.Engine.Utility;
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
        public Generator generator { get; private set; }
        public List<TileLayer> layers { get; set; }
        public Dictionary<string, TileLayer> layersByName = new Dictionary<string, TileLayer>();
        public Dictionary<Vector2, Region> regions { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int widthInPixels { get; set; }
        public int heightInPixels { get; set; }
        public int regionTilesWide { get; set; } = 8;
        public int regionTilesHigh { get; set; } = 8;

        public ConcurrentDictionary<int, Rectangle> collidables { get; set; }

        public ConcurrentDictionary<int, Entity> entities { get; set; }
        public ConcurrentDictionary<int, LivingEntity> livingEntities { get; set; }

        public ConcurrentDictionary<int, Spawn> spawns { get; set; }
        public double spawnRate;
        public int livingEntityCap = 50;
        public Timer spawnTimer;

        private long v_drawnTileCount;

        public List<Region> v_regionsVisible = new List<Region>();
        private List<Tile> v_TileTemplates = new List<Tile>();
        private Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>> v_VisibleTiles = new Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>>();

        private List<Graphic> v_TileEdges = new List<Graphic>();
        private Dictionary<TileLayer, Dictionary<Graphic, List<Vector2>>> v_VisibleEdges = new Dictionary<TileLayer, Dictionary<Graphic, List<Vector2>>>();

        private Rectangle v_CameraViewBox = new Rectangle();


        public Dictionary<Vector2, ItemEntity> items = new Dictionary<Vector2, ItemEntity>();

        //====================================================================================
        // CONSTRUCTOR
        //====================================================================================
        public Map(DataPack pack, YamlSection config)
        {
            name = config.getName();
            string genName = config.getString("generator");
            generator = GeneratorManager.getByNamespace(genName);
            if (generator == null)
            {
                Console.WriteLine($"// ERR: No generator found with name '{genName}'!");
                MapManager.remove(this);
                return;
            }

            this.width = config.getInt("width", 384);
            this.height = config.getInt("height", 384);
            this.layers = generator.generateLayers(width);
            this.widthInPixels = width * TileManager.dimensions;
            this.heightInPixels = height * TileManager.dimensions;
            regions = new Dictionary<Vector2, Region>();
            collidables = new ConcurrentDictionary<int, Rectangle>();

            this.entities = new ConcurrentDictionary<int, Entity>();
            this.livingEntities = new ConcurrentDictionary<int, LivingEntity>();
            this.spawns = new ConcurrentDictionary<int, Spawn>();
            spawnRate = config.getDouble("spawn_rate", 1);
            initSpawns();
            spawnTimer = new Timer(spawnRate * 1000);
            spawnTimer.Elapsed += trySpawn;
            spawnTimer.Start();
            livingEntityCap = config.getInt("entity_cap", 50);

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
                // Ensure this layer is in the "ByName" dictionary
                layersByName.Add(layer.name, layer);

                Dictionary<Vector2, Tile> tiles = layer.tiles;
                foreach (KeyValuePair<Vector2, Tile> pair in tiles)
                {
                    Vector2 pos = pair.Key;
                    Tile tile = pair.Value;

                    regionPos = new Vector2((int)(tile.tilePos.X / 8), (int)(tile.tilePos.Y / 8));
                    tile.region = regionPos;
                    tile.map = this;  // Tile remembers the Map it belongs to
                    tile.layer = layer;  // Tile remembers Map's Layer it belongs to
                    if (!regions.ContainsKey(regionPos)) continue;
                    regions[regionPos].addTile(tile);

                    tile.update();

                    //if (!tile.isCollidable) continue;
                    // Create a collidable box at the following true-map coordinate.
                    //collidables.TryAdd(collidables.Count, new Rectangle(Convert.ToInt32(tile.pos.X), Convert.ToInt32(tile.pos.Y), TileManager.dimensions, TileManager.dimensions));
                }

            }

            Save.oldMapStates.Add(name, new Map(this));
            buildTileTemplateCache();
        }
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
                // Ensure this layer is in the "ByName" dictionary
                layersByName.Add(layer.name, layer);

                Dictionary<Vector2, Tile> tiles = layer.tiles;
                foreach (KeyValuePair<Vector2, Tile> pair in tiles)
                {
                    Vector2 pos = pair.Key;
                    Tile tile = pair.Value;

                    regionPos = new Vector2((int)(tile.tilePos.X / 8), (int)(tile.tilePos.Y / 8));
                    tile.region = regionPos;
                    tile.map = this;  // Tile remembers the Map it belongs to
                    tile.layer = layer;  // Tile remembers Map's Layer it belongs to
                    if (!regions.ContainsKey(regionPos)) continue;
                    regions[regionPos].addTile(tile);

                    tile.update();

                    //if (!tile.isCollidable) continue;
                    // Create a collidable box at the following true-map coordinate.
                    //collidables.TryAdd(collidables.Count, new Rectangle(Convert.ToInt32(tile.pos.X), Convert.ToInt32(tile.pos.Y), TileManager.dimensions, TileManager.dimensions));
                }

            }

            Save.oldMapStates.Add(name, new Map(this));
            buildTileTemplateCache();

        }

        // COnstruct a map based on an existing map.
        public Map(Map oldMap)
        {
            this.name = oldMap.name;
            this.layers = oldMap.layers;
            this.width = oldMap.width;
            this.height = oldMap.height;
            this.widthInPixels = oldMap.widthInPixels;
            this.heightInPixels = oldMap.height;
            regions = new Dictionary<Vector2, Region>(oldMap.regions);
            collidables = new ConcurrentDictionary<int, Rectangle>(oldMap.collidables);

            this.entities = new ConcurrentDictionary<int, Entity>(oldMap.entities);
            this.livingEntities = new ConcurrentDictionary<int, LivingEntity>(oldMap.livingEntities);
            this.spawns = new ConcurrentDictionary<int, Spawn>(oldMap.spawns);

        }


        //====================================================================================
        // PROPERTIES
        //====================================================================================

        // Total # of Tiles on entire Map
        public long TilesTotalCount
        {
            get { return layers.Count; }
        }

        // Tracks # of Tiles drawn last frame
        public long TilesDrawnCount
        {
            get { return v_drawnTileCount; }
            set { v_drawnTileCount = value; }
        }

        // Calculates the number of Cached Tiles (visible for drawing)
        public long TilesCachedCount
        {
            get
            {
                long tCount = 0;

                // Go through each CachedTiles Layer
                foreach (TileLayer tLayer in layers)
                {
                    // Go through each Tile Template's Locations List in the Layer
                    foreach (List<Vector2> locations in v_VisibleTiles[tLayer].Values)
                    {
                        // Get Sub-Tile Locations
                        tCount += locations.Count;
                    }
                }

                return tCount;
            }

        }

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

        /// <summary>
        /// Get a layer from the map by its name.
        /// </summary>
        /// <param name="name">The name of the layer.</param>
        /// <returns>The layer matching "name"; null if not found.</returns>
        public TileLayer getLayer(string name)
        {
            if (name != null && layersByName.ContainsKey(name))
                return layersByName[name];

            return null;
        }
        public int getRegionSizeWideInPixels() { return regionTilesWide * TileManager.dimensions; }

        public int getRegionSizeHighInPixels() { return regionTilesHigh * TileManager.dimensions; }


        //====================================================================================
        // ENTITY FUNCTIONS
        //====================================================================================

        /// <summary>
        /// Set up the spawn chances for varying entities on this map.
        /// </summary>
        public void initSpawns()
        {
            //spawns.TryAdd(spawns.Keys.Count, new Spawn(EntityManager.get<LivingEntity>(0), 2));
            //spawns.TryAdd(spawns.Keys.Count, new Spawn(EntityManager.get<LivingEntity>(1), 3));
            //spawns.TryAdd(spawns.Keys.Count, new Spawn(EntityManager.get<LivingEntity>(2), 1));
        }
        /// <summary>
        /// Try to spawn an entity on the map.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void trySpawn(Object source, ElapsedEventArgs e)
        {
            if (spawns.Count == 0) return;
            if (livingEntities.Count >= livingEntityCap) return;
            Random rand = new Random();

            if (rand.Next(0, 100) >= 25) return;

            Spawn spawn = Utility.Util.randomizeSpawn(spawns);

            //System.Diagnostics.Debug.WriteLine("Successfully spawned entity " + spawn.entity.name);

            Vector2 target = findSpawnLocation(spawn.entity);

            LivingEntity ent = new LivingEntity(spawn.entity, target, livingEntities.Count, this);
            livingEntities.TryAdd(livingEntities.Keys.Count, ent);

        }
        /// <summary>
        /// Attempts to find a place to spawn an entity.
        /// </summary>
        /// <param name="ent">The entity we are trying to spawn.</param>
        /// <returns>The safe position found.</returns>
        public Vector2 findSpawnLocation(LivingEntity ent)
        {
            Vector2 pos = new Vector2();

            Random rand = new Random();

            int x = rand.Next(32, widthInPixels - 32);
            int y = rand.Next(32, heightInPixels - 32);

            pos.X = x;
            pos.Y = y;

            // TODO: Implement a "max tries" to prevent endless recursion.
            Rectangle target = new Rectangle(x, y, ent.boundingBox.Width, ent.boundingBox.Height);
            if (!isLocationSafe(target))
            {
                return findSpawnLocation(ent);
            }

            return pos;
        }
        /// <summary>
        /// Checks if a position is safe to place an object in.
        /// </summary>
        /// <param name="location">The bounding box </param>
        /// <returns></returns>
        public bool isLocationSafe(Rectangle location)
        {
            Vector2 tilePos = Utility.Util.getTilePosition(new Vector2(location.X, location.Y));
            List<Tile> nearbyTiles = Utility.Util.getSurroundingTiles(this, 2, tilePos);

            Rectangle collidable;
            foreach (Tile tile in nearbyTiles)
            {
                if (tile == null) continue;
                if (!tile.isCollidable) continue;

                collidable = tile.box;
                collidable.Inflate(5, 5);
                if (location.Intersects(collidable)) return false;
            }
            return true;
        }
        /// <summary>
        /// Generates an item entity using an item object and location.
        /// </summary>
        /// <param name="item">The item instance.</param>
        /// <param name="pos">The true map position.</param>
        public void spawnItem(Item item, Vector2 pos)
        {
            pos.X = (int)pos.X;
            pos.Y = (int)pos.Y;
            if (!items.ContainsKey(pos))
                new ItemEntity(this, item, pos);
            else
                spawnItem(item, new Vector2(pos.X + 1, pos.Y)); // Kind of dirty fix... 
        }

        //====================================================================================
        // TILE MAP FUNCTIONS
        //====================================================================================
        /// <summary>
        /// Retrieve a region using a tile.
        /// </summary>
        /// <param name="tile">The tile we are retrieving the region of.</param>
        /// <returns>The region this tile resides in.</returns>
        public Region getRegionByTile(Tile tile)
        {
            return regions[tile.region];
        }
        /// <summary>
        /// Retrieve a region base on a tile position.
        /// </summary>
        /// <param name="tilePos">The position of the tile.</param>
        /// <returns>The region the provided tile position resides in.</returns>
        public Region getRegionByTilePosition(Vector2 tilePos)
        {
            Vector2 regionPos = new Vector2((int)(tilePos.X / 8), (int)(tilePos.Y / 8));

            return regions[regionPos];
        }
        /// <summary>
        /// Get regions surrounding the region a tile belongs to.
        /// </summary>
        /// <param name="tilePos">The position of the tile we are using as our starting point.</param>
        /// <param name="radius">How many regions around we want to retrieve.</param>
        /// <returns>A list of regions found nearby.</returns>
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
        // Gets the top-most layer.
        public TileLayer getTopLayer()
        {
            return layers[layers.Count - 1];
        }
        // Gets the tile at a position from the top-most layer.
        public Tile getTopTile(Vector2 tilePos)
        {
            List<TileLayer> layers = new List<TileLayer>(this.layers);
            layers.Reverse();
            Tile tile = null;

            // Loop through the layers in reverse order.
            foreach (TileLayer layer in layers)
            {
                tile = layer.getTile(tilePos);

                // If the tile isn't null, we've found the top-most tile.
                if (tile != null) break;
            }

            return tile;
        }
        public Tile getTile(TileLayer mLayer, Vector2 mPosition)
        {
            if (mLayer.tiles.ContainsKey(mPosition)) { return mLayer.tiles[mPosition]; }
            // Otherwise...
            return null;
        }

        // Remove Tile from Layer and Position
        public bool removeTile(TileLayer mLayer, Vector2 mTilePosition)
        {
            Tile tile;


            if (!mLayer.tiles.ContainsKey(mTilePosition))
            {
                //Utility.Util.myDebug("No Tile on Decorations Layer at position: " + mTilePosition);
                return false;
            }
            // Otherwise...

            // Get Tile
            tile = mLayer.tiles[mTilePosition];

            // Remove Tile from Map
            this.removeTile(tile);
            //Utility.Util.myDebug("Tile REMOVED on Decorations Layer at position: " + mTilePosition);

            return true;
        }
        // Remove specific Tile from Map (and Caches)
        public bool removeTile(Tile mTile)
        {
            // Remove Tile from Layer
            TileLayer layer = mTile.layer;

            // If Tile does NOT exist on Map  (return FALSE for Removing sent Tile)
            if (!layer.tiles.ContainsValue(mTile)) { return false; }
            // Otherwise...

            layer.clearTile(mTile.tilePos);

            // Remove Tile from Region
            Region region = regions[mTile.region];
            region.removeTile(mTile);

            return true;
        }
        // Adds a new Tile to the Map
        public bool addTile(Tile mTile)
        {

            Vector2 pos = mTile.pos;
            Region region;


            // If NO Layer given
            if (mTile.layer == null)
            {
                // Do NOT Add Tile to Map. Tile had no Layer information.
                Utility.Util.myDebug(true, "Map.cs addTile(Tile)", "Could NOT Add Tile(" + mTile.name + "). Tile had no assigned Layer.");
                return false;
            }
            // Otherwise continue...

            // Check if Tile already exists at same Position and Layer
            if (mTile.layer.tiles.ContainsKey(mTile.pos))
            {
                // Do NOT Add Tile to Map. A Tile already exists at that Position
                Utility.Util.myDebug(true, "Map.cs addTile(Tile)", "Could NOT Add Tile(" + mTile.name + "). A Tile already exists at Layer(" + this.name + ") position: " + mTile.pos);
                return false;
            }
            // Otherwise continue...


            mTile.map = this;

            // If didn't work... Return FALSE.
            if (!mTile.layer.addTile(mTile)) { return false; }
            // Otherwise...

            // Add Tile to proper Region
            // Figure out matching Region (based on Tile Position)

            Vector2 regionPos;

            // Calculate what Region Position the Tile would fall into
            regionPos.X = (int)(mTile.tilePos.X / regionTilesWide);
            regionPos.Y = (int)(mTile.tilePos.Y / regionTilesHigh);

            // Get that Region
            region = regions[regionPos];

            // Add mTile to Region
            region.tiles.Add(mTile);
            // Assign Tile's Region value to this Region
            mTile.region = regionPos;


            // Update Tile rendering Cache
            // Update Visible Tiles if Tile belongs to any Visible Region
            if (v_regionsVisible.Contains(region)) { buildVisibleTileCache(); }

            return true;
        }


        //====================================================================================
        // RENDER AND CACHE FUNCTIONS
        //====================================================================================
        public void update_VisibleRegions (Camera2D camera)
        {

            // Clear Visible Regions collection
            v_regionsVisible.Clear();


            // Update stored Camera ViewBox Rectangle to match new/current Camera viewbox
            // Useful for future planned (only update on significant changes system)
            v_CameraViewBox.X = camera.BoundingRectangle.X;
            v_CameraViewBox.Y = camera.BoundingRectangle.Y;
            v_CameraViewBox.Width = camera.BoundingRectangle.Width;
            v_CameraViewBox.Height = camera.BoundingRectangle.Height;


            // Go through each Region on Map  (to re-populate Visible Regions collection)
            foreach (Region region in regions.Values)
            {

                // If THIS Region is INSIDE Camera's view (BoundingRectangle)
                if (v_CameraViewBox.Intersects(region.box))
                //if (camera.BoundingRectangle.Intersects(region.box))
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
            }

            //Utility.Util.myDebug("Visible Regions of Total:  " + VisibleRegions.Count + " / " + regions.Count);


            // Build Tile Cache (including Edges) for Drawing Map
            buildVisibleTileCache();

        }

        public void DrawVisibleMapCache (Camera2D camera, SpriteBatch batch)
        {

            // Go through each CachedTiles Layer
            batch.Begin(transformMatrix: Camera.camera.Transform);
            foreach (TileLayer tLayer in layers)
            {

                // ------------------------------------------------------------------
                // DRAW TILES - By Cached Tile Parent (Template)
                // ------------------------------------------------------------------
                // Get Template Tile's Cached Visible Map Tile List of Locations to draw to
                Dictionary<Tile, List<Vector2>> templateList = v_VisibleTiles[tLayer];

                // Go through each Tile Template in the Layer
                foreach (KeyValuePair<Tile, List<Vector2>> pair2 in templateList)
                {
                    pair2.Key.graphic.draw_Tiles(batch, pair2.Value);

                    v_drawnTileCount += pair2.Value.Count;  // Count drawn Tiles
                }

                // ------------------------------------------------------------------
                // DRAW EDGES - By Cached Edge Graphics
                // ------------------------------------------------------------------

                Dictionary<Graphic, List<Vector2>> edgeList = v_VisibleEdges[tLayer];
                // Go through all Visible Tile Edges to draw
                foreach (KeyValuePair<Graphic, List<Vector2>> pair2 in edgeList)
                {
                    // Draw ALL matching Visible Edges at once
                    pair2.Key.draw_Tiles(batch, pair2.Value);

                    v_drawnTileCount += pair2.Value.Count;  // Count drawn Tiles
                }

                // Loop through regions to draw tile edges and highlights
                foreach (Region region in v_regionsVisible)
                {
                    region.draw(batch, tLayer);
                }
            }
            batch.End();

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

        // Stores what Tile Templates this Map uses
        public void buildTileTemplateCache()
        {

            // Clear the Collection
            v_TileTemplates.Clear();

            // Go through map Layers
            foreach (TileLayer layer in layers)
            {
                // Go through Tiles in Layer
                foreach (Tile tile in layer.tiles.Values)
                {
                    // If Collection does NOT contain this Tile Parent (template)
                    if (!v_TileTemplates.Contains(tile.parent))
                    {
                        // Add this Parent Tile to Cache
                        v_TileTemplates.Add(tile.parent);
                    }
                }
            }


            // Prepare Visible Tile Caches for use  (will still need to be filled with "buildVisibleTileCache()")
            // Sets up Edge Tiles and Visible Tiles to work based on Time Templates
            setupVisibleTileCaches();
        }
        public void setupVisibleTileCaches()
        {
            // Clear Visible Tile Collections (just in case)
            v_VisibleTiles.Clear();
            v_VisibleEdges.Clear();


            // Generate all Edge Templates
            // Go through each Tile Template on This Map
            foreach (Tile tile in v_TileTemplates)
            {
                // Go through Edges (if any)
                foreach (KeyValuePair<TileSide, Graphic> pair in tile.sideGraphics)
                {
                    TileSide side = pair.Key;
                    Graphic graphic = pair.Value;

                    if (graphic != null)
                    {
                        // If this Graphic does NOT already exist in Collection
                        if (!v_TileEdges.Contains(graphic))
                        {
                            // Add Graphic to List
                            v_TileEdges.Add(graphic);
                        }
                    }
                }
            }


            // Create top most level of CachedTiles collection (the Map Layer Names), in order of appearance in Map.layers
            // Layer Name is first Level of multi-dimensional v_TileCache collection
            foreach (TileLayer tileLayer in layers)
            {
                // Add New Empty Item to Collection for this Layer with Layer as Key
                v_VisibleTiles.Add(tileLayer, new Dictionary<Tile, List<Vector2>>());
                v_VisibleEdges.Add(tileLayer, new Dictionary<Graphic, List<Vector2>>());
            }
        }

        public void buildVisibleTileCache()
        {

            Dictionary<Tile, List<Vector2>> tileTemplate;
            Tile tile;
            Vector2 pos = new Vector2();
            int layerIndex = 0;
            //TileLayer layer;

            Clear_ChildTiles();

            // Get Tile Bounds
            Rectangle tileViewBounds = new Rectangle();

            tileViewBounds.X = (v_CameraViewBox.X / TileManager.dimensions) - 2;
            tileViewBounds.Y = (v_CameraViewBox.Y / TileManager.dimensions) -2;
            tileViewBounds.Width = (v_CameraViewBox.Width / TileManager.dimensions) + 4;
            tileViewBounds.Height = (v_CameraViewBox.Height / TileManager.dimensions) + 4;

            // Go through each Layer on Map
            foreach (TileLayer layer in v_VisibleTiles.Keys)
            {
                // Get Tile Template Dictionary matching Layer
                tileTemplate = v_VisibleTiles[layer];

                for (int X = tileViewBounds.X; X <= tileViewBounds.Right; X++)
                {
                    for (int Y = tileViewBounds.Y; Y <= tileViewBounds.Bottom; Y++)
                    {
                        pos.X = X;
                        pos.Y = Y;

                        // Get Tile
                        tile = getTile(layer, pos);
                        if (tile == null) { continue; }
                        // Otherwise...

                        // If tileTemplate Dictionary does NOT have this Tile Template already
                        if (!tileTemplate.ContainsKey(tile.parent))
                        {

                            // Add Tile template with this Tile's Position to Collection
                            tileTemplate.Add(tile.parent, new List<Vector2>());

                        }


                        // Check if any Tiles ABOVE This tile completely cover it
                        bool keepTile = true;

                        for (int checkLayer = layerIndex + 1; checkLayer < layers.Count; checkLayer++)
                        {
                            // If a Tile does NOT exist at this Position on next Layer
                            if (!layers[checkLayer].tiles.ContainsKey(pos)) { continue; }
                            // Otherwise...

                            // If checkTile is on Decorations Layer (it's probably see through, so ignore this Layer)
                            if (layers[checkLayer].name == "decorations") { continue; }
                            // Otherwise...


                            Tile checkTile = layers[checkLayer].getTile(pos);

                            // If checkTile is NOT See Through  (will completely cover current tile being checked for)
                            if (!checkTile.seeThrough)
                            {
                                keepTile = false;
                                break;
                            }

                        }

                        // Store this Tile for drawing
                        if (keepTile) { tileTemplate[tile.parent].Add(tile.drawPos); }


                        // Go through Edges (if any)
                        foreach (KeyValuePair<TileSide, bool> pair in tile.sides)
                        {

                            if (!pair.Value) continue;

                            TileSide side = pair.Key;
                            Graphic graphic = tile.sideGraphics[side];
                            Vector2 drawPos = new Vector2(tile.drawPos.X, tile.drawPos.Y);
                            int tileSize = TileManager.dimensions;

                            if (graphic != null)
                            {
                                if (v_TileEdges.Contains(graphic))
                                {
                                    switch (side)
                                    {
                                        case TileSide.NorthWest:
                                            drawPos.X -= tileSize;
                                            drawPos.Y -= tileSize;
                                            break;
                                        case TileSide.North:
                                            drawPos.Y -= tileSize;
                                            break;
                                        case TileSide.NorthEast:
                                            drawPos.X += tileSize;
                                            drawPos.Y -= tileSize;
                                            break;
                                        case TileSide.West:
                                            drawPos.X -= tileSize;
                                            break;
                                        case TileSide.East:
                                            drawPos.X += tileSize;
                                            break;
                                        case TileSide.SouthWest:
                                            drawPos.X -= tileSize;
                                            drawPos.Y += tileSize;
                                            break;
                                        case TileSide.South:
                                            drawPos.Y += tileSize;
                                            break;
                                        case TileSide.SouthEast:
                                            drawPos.X += tileSize;
                                            drawPos.Y += tileSize;
                                            break;
                                    }
                                    if (!v_VisibleEdges[layer].ContainsKey(graphic))
                                        v_VisibleEdges[layer].Add(graphic, new List<Vector2>());

                                    v_VisibleEdges[layer][graphic].Add(drawPos);
                                }
                                else
                                {
                                    Utility.Util.myDebug(true, "Map.cs buildVisibleTileCache()", "Missing graphic from (v_EdgeTiles). Tile: " + tile.name);
                                }
                            }
                        }

                    }
                }

                layerIndex++;
            }

        }
        public void Clear_ChildTiles()
        {
            // Visible Tiles
            // For each Template Tile
            foreach (Dictionary<Tile, List<Vector2>> template in v_VisibleTiles.Values)
            {
                foreach (List<Vector2> list in template.Values)
                {
                    list.Clear();
                }
            }

            // Visible Edge Tiles
            // For each Template Tile
            foreach (Dictionary<Graphic, List<Vector2>> template in v_VisibleEdges.Values)
            {
                foreach (List<Vector2> list in template.Values)
                {
                    list.Clear();
                }
            }
        }


        //====================================================================================
        // DISPOSAL AND GC
        //====================================================================================

        public void Clear()
        {
            // Clear Tile Caches
            this.v_VisibleTiles.Clear();
            this.v_VisibleEdges.Clear();
            this.v_TileEdges.Clear();

            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }


    }
}
