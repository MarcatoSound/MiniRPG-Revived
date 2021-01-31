using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using Newtonsoft.Json.Linq;
using RPGEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        public List<TileLayer> layers { get; set; } = new List<TileLayer>();
        public Dictionary<Vector2, Region> regions { get; set; } = new Dictionary<Vector2, Region>();
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

        public bool DrawRegionBorders { get; set; } = true;
        public int regionTilesWide { get; set; } = 8;
        public int regionTilesHigh { get; set; } = 8;

        private long v_drawnTileCount;

        public List<Region> VisibleRegions = new List<Region>();
        private List<Tile> v_TileTemplates = new List<Tile>();
        private Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>> v_VisibleTiles = new Dictionary<TileLayer, Dictionary<Tile, List<Vector2>>>();

        private Dictionary<Graphic, List<Vector2>> v_TileEdges = new Dictionary<Graphic, List<Vector2>>();
        private Dictionary<Graphic, List<Vector2>> v_VisibleEdges = new Dictionary<Graphic, List<Vector2>>();

        private Rectangle v_RegionViewBox = new Rectangle();
        private Rectangle v_CameraViewBox = new Rectangle();


        private Rectangle v_TileViewBox = new Rectangle();
        private Tile[,,] v_TileCache { get; set; }


        //====================================================================================
        // CONSTRUCTOR
        //====================================================================================
        public Map(string world, string name, int width, int height)
        {
            this.name = name;
            this.layers = layers;
            this.width = width;
            this.height = height;
            this.widthInPixels = width * TileManager.dimensions;
            this.heightInPixels = height * TileManager.dimensions;
            collidables = new ConcurrentDictionary<int, Rectangle>();

            this.entities = new ConcurrentDictionary<int, Entity>();
            this.livingEntities = new ConcurrentDictionary<int, LivingEntity>();
            this.spawns = new ConcurrentDictionary<int, Spawn>();
            initSpawns();
            spawnTimer = new Timer(1000);
            spawnTimer.Elapsed += trySpawn;
            spawnTimer.Start();


            this.loadMap(world, name);

            this.buildMapRegions();

            /*
            regions = new Dictionary<Vector2, Region>();

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
            */


            buildTileTemplateCache();
            //generateTileEdgesCache();

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


        public Tile getTile (TileLayer mLayer, Vector2 mPosition)
        {
            if (mLayer.tiles.ContainsKey(mPosition)) { return mLayer.tiles[mPosition]; }
            // Otherwise...
            return null;
        }

        public Tile getDecorationTile (Vector2 mPosition)
        {
            foreach (TileLayer layer in layers)
            {
                if (layer.name == "decorations")
                {
                    if (layer.tiles.ContainsKey(mPosition)) { return layer.tiles[mPosition]; }
                }
            }

            return null;
        }


        public TileLayer getLayerByName (string mLayerName)
        {
            foreach (TileLayer layer in layers)
            {
                if (layer.name == mLayerName)
                {
                    return layer;
                }
            }

            return null;
        }


        public int getRegionSizeWideInPixels () { return regionTilesWide * TileManager.dimensions; }

        public int getRegionSizeHighInPixels () { return regionTilesHigh* TileManager.dimensions; }



        //====================================================================================
        // FUNCTIONS - Map Setup
        //====================================================================================

        // LOAD MAP
        public void loadMap(string world, string map)
        {

            // Track how long this function takes to run
            Utility.CodeTimer codeTimer = new Utility.CodeTimer();
            codeTimer.startTimer();


            //List<TileLayer> layers = new List<TileLayer>();

            string path = $"save\\{world}\\maps";
            StreamReader reader = new StreamReader($"save\\{world}\\world.json");

            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles();

            // Declare variables here instead of repeatedly in For loops below
            TileLayer layer;
            int layerIndex = -1;
            JArray tileArray;
            List<Tile> tiles;
            Tile template;
            int templateID;
            Tile tile;


            // Each File is a Map Tile Layer
            foreach (FileInfo file in files)
            {
                layerIndex += 1;  // Count Layer Index

                // TODO: Account for incorrect layer loading order.
                reader = new StreamReader($"{path}\\{file.Name}");
                JObject jsonLayer = JObject.Parse(reader.ReadToEnd());

                layer = new TileLayer(this, jsonLayer.Value<string>("layer"), layerIndex);
                tileArray = jsonLayer.Value<JArray>("tiles");
                tiles = new List<Tile>();


                // Go through each Tile in this Layer
                foreach (JObject tileJson in tileArray)
                {
                    templateID = tileJson.Value<int>("id");
                    // Grab Template Tile
                    template = TileManager.get(templateID);

                    // If NO Template Tile given, skip  (it's bad)
                    if (template == null)
                    {
                        // Report Error
                        Utility.Util.myDebug(true, "Map.cs loadMap()", "Missing Tile Template (ID: " + templateID + ") on Map Tile (" + tileJson.Value<int>("x") + ", " + tileJson.Value<int>("y") + ")");
                        continue;  // Skip this Tile  (it's bad)
                    }
                    // Otherwise...

                    int x = tileJson.Value<int>("x");
                    int y = tileJson.Value<int>("y");

                    tile = new Tile(template, new Vector2(x, y));

                    layer.setTile(tile.tilePos, tile);
                    tile = null;
                }


                layers.Add(layer);
                System.Diagnostics.Debug.WriteLine("Loaded layer: " + layer.name);

                reader.Close();

                layer = null;

                GC.Collect();

            }


            // Report Load Speed
            codeTimer.endTimer();
            // Report function's speed
            Utility.Util.myDebug("Load.cs loadMap()", "Map (" + path + ") Loaded CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());


            //return layers;

        }


        private void buildMapRegions ()
        {
            // Create Regions
            regions = new Dictionary<Vector2, Region>();
            //List<Tile> tiles = new List<Tile>();

            int regionsWideInTiles = width / regionTilesWide;
            int reigionsHighInTiles = height / regionTilesHigh;

            Vector2 regionTruePos = new Vector2();
            Vector2 regionPos = new Vector2();

            // Create Regions and their Map Positions
            for (int x = 0; x < regionsWideInTiles; x++)
            {
                for (int y = 0; y < reigionsHighInTiles; y++)
                {
                    regionTruePos.X = x * (TileManager.dimensions * regionTilesWide);
                    regionTruePos.Y = y * (TileManager.dimensions * regionTilesHigh);
                    regionPos.X = x;
                    regionPos.Y = y;
                    
                    Region region = new Region(this, regionTruePos, regionPos);
                    regions.Add(new Vector2(x, y), region);

                    /*
                    // Add Tiles to Region
                    foreach (TileLayer layer in layers)
                    {
                        // Get Tiles from within this Region's bounds
                        List<Tile> tiles = this.getTilesInBox(layer, new Rectangle((int)regionPos.X, (int)regionPos.Y, regionTilesWide, regionTilesHigh));

                        foreach (Tile tile in tiles)
                        {
                            if (tile == null) continue;
                            // Otherwise...

                            //tile.region = new Vector2((int)(tile.tilePos.X / regionTilesWide), (int)(tile.tilePos.Y / regionTilesHigh));
                            tile.region = regionPos;
                            tile.map = this;  // Tile remembers the Map it belongs to
                            tile.layer = layer;  // Tile remembers Map's Layer it belongs to
                            region.addTile(tile);
                            
                            tile.update();

                            if (layer.name == "water") continue;
                            if (!tile.isCollidable) continue;

                            // Create a collidable box at the following true-map coordinate.
                            collidables.TryAdd(collidables.Count, new Rectangle(Convert.ToInt32(tile.pos.X), Convert.ToInt32(tile.pos.Y), TileManager.dimensions, TileManager.dimensions));

                        }

                    }

                    region.buildTileCache();
                    */
                }
            }

            // Fill Regions with the Tiles they contain
            // Go through each TileLayer on Map
            foreach (TileLayer layer in layers)
            {
                Dictionary<Vector2, Tile> tiles = layer.tiles;
                
                // Go through all Tiles in this TileLayer
                foreach (KeyValuePair<Vector2, Tile> pair in tiles)
                {
                    Vector2 pos = pair.Key;
                    Tile tile = pair.Value;

                    regionPos = new Vector2((int)(tile.tilePos.X / regionTilesWide), (int)(tile.tilePos.Y / regionTilesHigh));
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


        public List<Region> getRegionsInRectangle (Rectangle mRectangle)
        {
            List<Region> collectedRegions = new List<Region>();

            int leftMostRegionX = mRectangle.X / getRegionSizeWideInPixels();
            int topMostRegionY = mRectangle.Y / getRegionSizeHighInPixels();
            int rightMostRegionX = mRectangle.Right / getRegionSizeWideInPixels();
            int bottomMostRegionY = mRectangle.Bottom / getRegionSizeHighInPixels();

            // Fix rounding up errors
            if ((float)leftMostRegionX > (mRectangle.X / getRegionSizeWideInPixels())) { leftMostRegionX = -1; }
            if ((float)rightMostRegionX > (mRectangle.Right / getRegionSizeWideInPixels())) { rightMostRegionX = -1; }
            if ((float)topMostRegionY > (mRectangle.Y / getRegionSizeHighInPixels())) { topMostRegionY = -1; }
            if ((float)bottomMostRegionY > (mRectangle.Bottom / getRegionSizeHighInPixels())) { bottomMostRegionY = -1; }


            for (int x = leftMostRegionX; x <= rightMostRegionX; x++)
            {
                for (int y = topMostRegionY; y <= bottomMostRegionY; y++)
                {
                    Vector2 pos = new Vector2(x, y);

                    collectedRegions.Add(regions[pos]);
                }
            }

            return collectedRegions;
        }

        public void update_VisibleRegionsTEST (Camera2D camera)
        {


            // Only update if need to

            /*
            bool update = false;

            if (v_CameraViewBox.X == 0)
            {
                if (v_CameraViewBox.Y == 0)
                {
                    update = true;
                }
            }

            if (!update)
            {
                //if (camera.BoundingRectangle.X < (v_CameraViewBox.X - (getRegionSizeWideInPixels() - TileManager.dimensions)))
                if (camera.BoundingRectangle.X < (v_CameraViewBox.X - (TileManager.dimensions * 2)))
                {
                    update = true;
                }

                if (camera.BoundingRectangle.X > (v_CameraViewBox.X + (TileManager.dimensions * 2)))
                {
                    update = true;
                }

                if (camera.BoundingRectangle.Y < (v_CameraViewBox.Y - (TileManager.dimensions * 2)))
                {
                    update = true;
                }

                if (camera.BoundingRectangle.Y > (v_CameraViewBox.Y + (TileManager.dimensions * 2)))
                {
                    update = true;
                }
            }

            // If do NOT update  (no need yet)... Quit function.
            if (!update) { return; }
            // Otherwise... Continue to update Visible Regions and Tiles Cache
            */

            // Clear Visible Regions collection
            VisibleRegions.Clear();


            // Update stored Camera ViewBox Rectangle to match new/current Camera viewbox
            // Useful for future planned (only update on significant changes system)
            v_CameraViewBox.X = camera.BoundingRectangle.X;
            v_CameraViewBox.Y = camera.BoundingRectangle.Y;
            v_CameraViewBox.Width = camera.BoundingRectangle.Width;
            v_CameraViewBox.Height = camera.BoundingRectangle.Height;

            // Use a slightly shrunken ViewPort instead  (full Camera BoundingRectangle is good, but for some reason grabs one X and Y Region row earlier than it should)
            v_RegionViewBox.X = v_CameraViewBox.X + getRegionSizeWideInPixels();
            v_RegionViewBox.Y = v_CameraViewBox.Y + getRegionSizeHighInPixels();
            v_RegionViewBox.Width = v_CameraViewBox.Width - getRegionSizeWideInPixels();
            v_RegionViewBox.Height = v_CameraViewBox.Height - getRegionSizeHighInPixels();


            //VisibleRegions = getRegionsInRectangle(camera.BoundingRectangle);
            VisibleRegions = getRegionsInRectangle(v_CameraViewBox);

            //List<Region> regionsTEST = getRegionsInRectangle(v_CameraViewBox);


            //Utility.Util.myDebug("Visible Regions of Total:  " + VisibleRegions.Count + " / " + regions.Count);


            // Build Tile Cache (including Edges) for Drawing Map
            buildVisibleTileCache();

        }


        public void update_VisibleRegions (Camera2D camera)
        {


            // Only update if need to

            /*
            bool update = false;

            if (v_CameraViewBox.X == 0)
            {
                if (v_CameraViewBox.Y == 0)
                {
                    update = true;
                }
            }

            if (!update)
            {
                //if (camera.BoundingRectangle.X < (v_CameraViewBox.X - (getRegionSizeWideInPixels() - TileManager.dimensions)))
                if (camera.BoundingRectangle.X < (v_CameraViewBox.X - (TileManager.dimensions * 2)))
                {
                    update = true;
                }

                if (camera.BoundingRectangle.X > (v_CameraViewBox.X + (TileManager.dimensions * 2)))
                {
                    update = true;
                }

                if (camera.BoundingRectangle.Y < (v_CameraViewBox.Y - (TileManager.dimensions * 2)))
                {
                    update = true;
                }

                if (camera.BoundingRectangle.Y > (v_CameraViewBox.Y + (TileManager.dimensions * 2)))
                {
                    update = true;
                }
            }

            // If do NOT update  (no need yet)... Quit function.
            if (!update) { return; }
            // Otherwise... Continue to update Visible Regions and Tiles Cache
            */

            // Clear Visible Regions collection
            VisibleRegions.Clear();


            // Update stored Camera ViewBox Rectangle to match new/current Camera viewbox
            // Useful for future planned (only update on significant changes system)
            v_CameraViewBox.X = camera.BoundingRectangle.X;
            v_CameraViewBox.Y = camera.BoundingRectangle.Y;
            v_CameraViewBox.Width = camera.BoundingRectangle.Width;
            v_CameraViewBox.Height = camera.BoundingRectangle.Height;

            // Use a slightly shrunken ViewPort instead  (full Camera BoundingRectangle is good, but for some reason grabs one X and Y Region row earlier than it should)
            v_RegionViewBox.X = v_CameraViewBox.X + getRegionSizeWideInPixels();
            v_RegionViewBox.Y = v_CameraViewBox.Y + getRegionSizeHighInPixels();
            v_RegionViewBox.Width = v_CameraViewBox.Width - getRegionSizeWideInPixels();
            v_RegionViewBox.Height = v_CameraViewBox.Height - getRegionSizeHighInPixels();


            //VisibleRegions = getRegionsInRectangle(camera.BoundingRectangle);
            //VisibleRegions = getRegionsInRectangle(v_CameraViewBox);

            //List<Region> regionsTEST = getRegionsInRectangle(v_CameraViewBox);


            // Go through each Region on Map  (to re-populate Visible Regions collection)
            foreach (Region region in regions.Values)
            {

                // If THIS Region is INSIDE Camera's view (BoundingRectangle)
                if (v_RegionViewBox.Intersects(region.box))
                //if (camera.BoundingRectangle.Intersects(region.box))
                {
                    // If List Does NOT Contain this Region
                    if (!VisibleRegions.Contains(region))
                    {
                        // Add this Region to Collection
                        VisibleRegions.Add(region);
                        
                        //Utility.Util.myDebug("Region Added:  " + region.box);

                        // TODO: Add Event for on Region ADDED to regionsVisible List
                    }

                }
            }

            //Utility.Util.myDebug("Visible Regions of Total:  " + VisibleRegions.Count + " / " + regions.Count);


            // Build Tile Cache (including Edges) for Drawing Map
            buildVisibleTileCache();

        }


        // Build Visible Tiles based on Regions in View
        public void buildVisibleTileCache2()
        {

            Clear_ChildTiles();


            /*
            // Clear Collection
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
                        if (!v_VisibleEdges.ContainsKey(graphic))
                        {
                            // Add Graphic and Tile Vector2 position to List
                            v_VisibleEdges.Add(graphic, new List<Vector2>());
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
            }

            */


            Dictionary<Tile, List<Vector2>> tileTemplate;
            TileLayer layer;

            // Go through Visible Regions
            foreach (Region region in VisibleRegions)
            {
                
                /*
                if (region.tilesCache.Count > 0)
                {
                    foreach (KeyValuePair<Tile, List<Vector2>> pair in region.tilesCache)
                    {
                        Tile key = pair.Key;
                        List<Vector2> value = pair.Value;

                        foreach (Vector2 pos in value)
                        {
                            //this.v_VisibleTiles[]
                        }
                    }
                }
                */
                
                
                // Go through each Tile in Region
                foreach (Tile tile in region.tiles)
                {
                    layer = tile.layer;
                    // Get Tile Template Dictionary matching Layer
                    tileTemplate = v_VisibleTiles[layer];


                    // If tileTemplate Dictionary does NOT have this Tile Template already
                    if (!tileTemplate.ContainsKey(tile.parent))
                    {
                        // Create List of Vector positions. List will contain Positions of ALL matching Tiles.
                        List<Vector2> list = new List<Vector2>();
                        // Add Position to TileCache List
                        list.Add(tile.drawPos);

                        // Add Tile template with this Tile's Position to Collection
                        tileTemplate.Add(tile.parent, list);

                    }
                    else
                    {
                        // Get Parent Tile's sub-tile position List
                        //List<Vector2> list = tileTemplate[tile.parent];
                        // Add Position to TileCache List
                        tileTemplate[tile.parent].Add(tile.drawPos);
                    }


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
                            if (v_VisibleEdges.ContainsKey(graphic))
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


                                v_VisibleEdges[graphic].Add(drawPos);
                            }
                            else
                            {
                                Utility.Util.myDebug(true, "Map.cs buildVisibleTileCache()", "Missing graphic from (v_EdgeTiles). Tile: " + tile.name );
                            }
                        }
                    }

                }
            }


            // Test for examining a target region
            //Region testRegion = regions[5, 5];
            //Utility.Util.myDebug("Test");

        }


        // Build Tile Cache for Drawing each Tile one by one
        public void buildVisibleTileCache3()
        {

            //Dictionary<Tile, List<Vector2>> tileTemplate;
            Tile tile;
            Vector2 pos = new Vector2();
            //TileLayer layer;
            int layerIndex = 0;

            Clear_ChildTiles();


            // Get Tile Bounds
            Rectangle tileViewBounds = new Rectangle();

            tileViewBounds.X = (v_CameraViewBox.X / TileManager.dimensions);
            tileViewBounds.Y = (v_CameraViewBox.Y / TileManager.dimensions);
            tileViewBounds.Width = (v_CameraViewBox.Width / TileManager.dimensions) + 2;
            tileViewBounds.Height = (v_CameraViewBox.Height / TileManager.dimensions) + 2;


            // Create empty Array of Tiles with proper Dimensions
            v_TileCache = new Tile[layers.Count, tileViewBounds.Right - tileViewBounds.X, tileViewBounds.Bottom - tileViewBounds.Y];

            int X2 = 0;
            int Y2 = 0;

            // Go through each Layer on Map
            foreach (TileLayer layer in layers)
            {
                // Skip Stone Layer (for testing)
                //if (layer.name == "stone") { continue; }


                for (int X = tileViewBounds.X; X < tileViewBounds.Right; X++)
                {
                    for (int Y = tileViewBounds.Y; Y < tileViewBounds.Bottom; Y++)
                    {
                        pos.X = X;
                        pos.Y = Y;

                        // Get Tile
                        tile = getTile(layer, pos);
                        if (tile == null) { continue; }
                        // Otherwise...


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

                        // Testing: Force keep ALL Tiles by setting to TRUE.
                        //keepTile = true;

                        // Store this Tile for drawing
                        if (keepTile) { v_TileCache[layerIndex, X2, Y2] = tile; }

                        Y2++;
                    }

                    Y2 = 0;
                    X2++;
                }

                X2 = 0;
                layerIndex++;
            }



            // Test for examining a target region
            //Region testRegion = regions[5, 5];
            //Utility.Util.myDebug("Test");

        }


        public void buildVisibleTileCache()
        {

            // For TESTING: Use buildVisibleTileCache3() instead.
            //buildVisibleTileCache3();
            //return;


            Dictionary<Tile, List<Vector2>> tileTemplate;
            Tile tile;
            Vector2 pos = new Vector2();
            int layerIndex = 0;
            //TileLayer layer;


            Clear_ChildTiles();


            // Get Tile Bounds
            Rectangle tileViewBounds = new Rectangle();

            tileViewBounds.X = (v_CameraViewBox.X / TileManager.dimensions);
            tileViewBounds.Y = (v_CameraViewBox.Y / TileManager.dimensions);
            tileViewBounds.Width = (v_CameraViewBox.Width / TileManager.dimensions) + 2;
            tileViewBounds.Height = (v_CameraViewBox.Height / TileManager.dimensions) + 2;


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
                            // Create List of Vector positions. List will contain Positions of ALL matching Tiles.
                            //List<Vector2> list = new List<Vector2>();
                            // Add Position to TileCache List
                            //list.Add(tile.drawPos);

                            // Add Tile template with this Tile's Position to Collection
                            //tileTemplate.Add(tile.parent, list);
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

                        // Testing: Force keep ALL Tiles by setting to TRUE.
                        //keepTile = true;

                        // Store this Tile for drawing
                        if (keepTile) { tileTemplate[tile.parent].Add(tile.drawPos); }

                        // Add this Tile's Position to Template Cache
                        //tileTemplate[tile.parent].Add(tile.drawPos);



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
                                if (v_VisibleEdges.ContainsKey(graphic))
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


                                    v_VisibleEdges[graphic].Add(drawPos);
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



            // Test for examining a target region
            //Region testRegion = regions[5, 5];
            //Utility.Util.myDebug("Test");

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


        public void setupVisibleTileCaches ()
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
                        if (!v_VisibleEdges.ContainsKey(graphic))
                        {
                            // Add Graphic and Tile Vector2 position to List
                            v_VisibleEdges.Add(graphic, new List<Vector2>());
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
            }
        }


        public List<Tile> getTilesInBox (TileLayer mLayer, Rectangle mRectangle)
        {
            // Return Layers with Tiles within sent Rectangle bounds

            // If NO Layer sent
            if (mLayer == null) { return null; }
            // Otherwise...

            List<Tile> tiles = new List<Tile>();
            Tile tile;


            int boundX1 = mRectangle.X;
            int boundX2 = mRectangle.X + mRectangle.Width;
            int boundY1 = mRectangle.Y;
            int boundY2 = mRectangle.Y + mRectangle.Height;

            // Fill Tiles in Layers
            for (int x = boundX1; x < boundX2; x++)
            {
                for (int y = boundY1; y < boundY2; y++)
                {
                    tile = mLayer.childTiles[x, y];
                    // Add this Tile to List
                    if (tile != null) { tiles.Add(tile); }
                    //tiles.Add(tile);
                }
            }


            // Return collected Tiles from Layer within bounds (Rectangle sent)
            return tiles;
        }


        public Dictionary<Tile, List<Vector2>> getCacheFromBox (TileLayer mLayer, Rectangle mRectangle)
        {

            if (mLayer == null) { return null; }
            // Otherwise...

            Dictionary<Tile, List<Vector2>> cache = new Dictionary<Tile, List<Vector2>>();
            Tile tile;


            int boundX1 = mRectangle.X;
            int boundX2 = mRectangle.X + mRectangle.Width;
            int boundY1 = mRectangle.Y;
            int boundY2 = mRectangle.Y + mRectangle.Height;

            // Fill Tiles in Layers
            for (int x = boundX1; x < boundX2; x++)
            {
                for (int y = boundY1; y < boundY2; y++)
                {
                    // Get Tile
                    tile = mLayer.childTiles[x, y];
                    // Get Parent Tile
                    Tile parent = tile.parent;

                    if (parent != null)
                    {
                        // If this Tile Template doesn't exist in Master Tiles Dictionary yet... Add it.
                        if (!cache.ContainsKey(tile.parent)) { cache.Add(tile.parent, new List<Vector2>()); }
                        // Store Position of this Child Tile in Master (Template) Tile Dictionary
                        cache[tile.parent].Add(tile.pos);
                    }
                }
            }

            return cache;
        }


        // Remove Decoration Tile at Position
        public void removeTile(Vector2 mTilePosition)
        {
            foreach (TileLayer layer in layers)
            {

                /*
                // For testing purposes...
                if (layer.tiles.ContainsKey(mTilePosition))
                {
                    // Get Tile
                    tile = layer.tiles[mTilePosition];

                    Utility.Util.myDebug("Layer (" + layer.name + "), Tile Position(" + mTilePosition + ") = " + tile.name);
                }
                */


                if (layer.name != "decorations") { continue; }
                // Otherwise...

                // Remove Tile from THIS Layer
                this.removeTile(layer, mTilePosition);

                break;
            }
        }

        // Remove Tile from Layer and Position
        public bool removeTile(TileLayer mLayer, Vector2 mTilePosition)
        {
            Tile tile;


            if (!mLayer.tiles.ContainsKey(mTilePosition))
            {
                Utility.Util.myDebug("No Tile on Decorations Layer at position: " + mTilePosition);
                return false;
            }
            // Otherwise...

            // Get Tile
            tile = mLayer.tiles[mTilePosition];

            // Remove Tile from Map
            this.removeTile(tile);
            Utility.Util.myDebug("Tile REMOVED on Decorations Layer at position: " + mTilePosition);

            return true;
        }

        // Remove specific Tile from Map (and Caches)
        public bool removeTile(Tile mTile)
        {

            // -----------------------------------------------------------------------------
            // VERIFICATIONS
            // -----------------------------------------------------------------------------

            if (mTile == null)
            {
                Utility.Util.myDebug(true, "Map.cs removeTile(Tile)", "Sent Tile is NULL");
                return false;
            }
            // Otherwise...


            // -----------------------------------------------------------------------------
            // MAP and LAYER
            // -----------------------------------------------------------------------------

            // Remove Tile from Layer
            TileLayer layer = mTile.layer;
            
            // If Tile does NOT exist on Map  (return FALSE for Removing sent Tile)
            if (!layer.tiles.ContainsValue(mTile)) { return false; }
            // Otherwise...
            
            layer.clearTile(mTile.tilePos);


            // Double check if Tile still exists
            //Tile testTile = layer.tiles[mTile.pos];

            // -----------------------------------------------------------------------------
            // REGION
            // -----------------------------------------------------------------------------

            // Remove Tile from Region
            Region region = regions[mTile.region];
            region.removeTile(mTile);


            // -----------------------------------------------------------------------------
            // CACHES and VISIBLE TILES
            // -----------------------------------------------------------------------------

            // TODO: Update Templates (if needed)
            Utility.Util.myDebug(true, "Map.cs removeTile(Tile)", "Update Template Caches if necessary! Not working yet!");


            // Update Visible Tiles
            if (this.VisibleRegions.Contains(region))
            {
                // Rebuilds updated Visible Tile Cache
                buildVisibleTileCache();
            }



            //this.buildTileTemplateCache();


            /*
            Dictionary<Tile, List<Vector2>> tileTemplates = tilesCache[mTile.layer];
            List<Vector2> list = tileTemplates[mTile.parent];
            list.Remove(mTile.pos);

            if (list.Count < 1)
            {
                // Remove TileTemplate from Cache
                tileTemplates.Remove(mTile.parent);
            }
            */

            Utility.Util.myDebug("Map.cs removeTile(Tile)", "Tile Removed!");

            return true;
        }


        // Adds a new Tile to the Map
        public bool addTile (Tile mTile)
        {

            Vector2 pos = mTile.pos;
            Region region;

            // -----------------------------------------------------------------------------
            // VERIFICATIONS
            // -----------------------------------------------------------------------------

            // If NO Layer given
            if (mTile.layer == null)
            {
                // Default to Layer 3 (decorations layer)
                //mTile.layer = layers[3];

                // OR

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


            // -----------------------------------------------------------------------------
            // UPDATE LAYER and MAP
            // -----------------------------------------------------------------------------

            mTile.map = this;

            // If didn't work... Return FALSE.
            if (!mTile.layer.addTile(mTile)) { return false; }
            // Otherwise...


            // -----------------------------------------------------------------------------
            // UPDATE REGION info
            // -----------------------------------------------------------------------------

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


            // -----------------------------------------------------------------------------
            // UPDATE CACHES
            // -----------------------------------------------------------------------------

            // TODO: Update Templates (if needed)
            //Utility.Util.myDebug(true, "Map.cs addTile(Tile)", "Update Template Caches if necessary! Not working yet!");

            // If Tile Template does NOT already exist in Caches
            if (!v_TileTemplates.Contains(mTile.parent))
            {
                // Update Tile Template collection
                v_TileTemplates.Add(mTile.parent);
                

                Utility.Util.myDebug("Map.cs addTile(Tile)", "Tile added to Templates: " + mTile.parent.name);
            }


            // -----------------------------------------------------------------------------
            // UPDATE EDGES of TILE
            // -----------------------------------------------------------------------------

            // Updates Edges
            mTile.update();
            
            // Add Edge Tiles to Edge Tile Templates if they don't exist
            // Go through each Edge Tile of mTile
            foreach (Graphic graphic in mTile.sideGraphics.Values)
            {
                // If this Edge Graphic does NOT exist in TileEdges collection
                if (!v_TileEdges.ContainsKey(graphic))
                {
                    // Add it to Collection
                    v_TileEdges.Add(graphic, new List<Vector2>());
                }
            }


            /*
            Vector2 testPos;
            Tile testTile;

            // Check bordering Tiles in every direction

            // NORTH EAST
            // Get tile at this Position (-1, -1) from This Tile.
            testPos.X = mTile.tilePos.X - 1;
            testPos.Y = mTile.tilePos.Y - 1;
            testTile = getTile(mTile.layer, mTile.tilePos);

            if (testTile != null)
            {
                if (testTile.name == "stone")
                {
                    mTile.sides.Add(TileSide.NorthEast, true);
                }
            }
            */


            // -----------------------------------------------------------------------------
            // UPDATE VISIBLE TILES
            // -----------------------------------------------------------------------------

            // Update Tile rendering Cache
            // Update Visible Tiles if Tile belongs to any Visible Region
            if (VisibleRegions.Contains(region)) 
            {
                // Update Visible Tiles
                buildVisibleTileCache();
            }


            return true;
        }

        /*
        // Modifies and Updates target Tile on Map  (and in all relevant Caches)
        public bool changeTile ( Tile mOldTile, Tile mNewTile )
        {

            TileLayer layer;

            // Remove mOldTile from all collections on Map
            // --------------------------------------------------------------------------------------------

            // Get target Tile's Layer
            layer = mOldTile.layer;

            // Remove Tile from Layer
            layer.tiles.Remove(mOldTile.pos);

            // If Old Tile Layer does NOT match New Tile Layer
            if (layer != mNewTile.layer)
            {
                // Replace Old Tile with Blank Tile
                // NOT NECESSARY  (Tiles only exist if they have something in them)
            }


            // Remove Tile from Caches

            // Remove Tile from Region Cache
            Region region = regions[mOldTile.region];
            region.tiles.Remove(mOldTile);
            // Get from Cache
            region.tilesCache.Remove(layer, mOldTile.p)

            // Re-Add mNewTile to Map in all relevant collections
            // --------------------------------------------------------------------------------------------

            layer = mNewTile.layer;

            



            return true;
        }
        */


        // NOT USED  (but might be helpful in the future)
        /*
        // Goes through entire Map and generates and stores all needed Edge Tiles and their Positions
        public void generateTileEdgesCache ()
        {

            v_EdgeTiles.Clear();

            // Generate Dictionary of TileEdges


            // Fill TileEdges Dictionary with valid Tile Location data (where to draw each Edge texture)

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
                        if (!v_EdgeTiles.ContainsKey(graphic))
                        {
                            // Add Graphic and Tile Vector2 position to List
                            v_EdgeTiles.Add(graphic, new List<Vector2>());
                        }
                    }
                }
            }

            // Fill Edge Templates with Vector2 position data of ALL matching Map Tiles

            foreach (TileLayer layer in layers)
            {
                foreach (Tile tile in layer.tiles.Values)
                {
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
                            if (v_EdgeTiles.ContainsKey(graphic))
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


                                v_EdgeTiles[graphic].Add(drawPos);
                            }
                            else
                            {
                                Utility.Util.myDebug(true, "Map.cs generateTileEdgesCache()", "Missing graphic from (v_EdgeTiles)");
                            }
                        }
                    }
                }
            }
        }
        */


        public void Draw3(Camera2D camera, SpriteBatch batch)
        {

            v_drawnTileCount = 0;

            batch.Begin(transformMatrix: Camera.camera.Transform, samplerState: SamplerState.PointClamp);


            foreach (Tile tile in v_TileCache)
            {
                if (tile == null) { continue; }
                // Otherwise...

                batch.Draw(tile.parent.tileSetTexture, tile.pos, tile.parent.textureRect, Color.White);
                //batch.Draw(tile.parent.tileSetTexture, tile.pos, tile.parent.textureRect, Color.White);
                //batch.Draw()
                //break;

                /*
                if (tile.parent.textureRect.X > 0)
                {
                    Utility.Util.myDebug("Tile Texture Rect NOT 0!");
                }
                */

                //tile.parent.graphic.draw(batch, tile.pos, 0f, Vector2.Zero, 1, false, 0.1f);
                //tile.drawAdjacentTiles(batch);

                // Count the tile
                v_drawnTileCount++;
            }


            // Draw Visible Regions Borders
            if (DrawRegionBorders)
            {
                foreach (Region region in VisibleRegions) { batch.DrawRectangle(region.box, Color.White); }
            }

            // Draw Test Red Square at Camera Bounds view  (for testing)
            //batch.DrawRectangle(new Rectangle(Camera.camera.BoundingRectangle.X + Camera.camera.BoundingRectangle.Width - 50, Camera.camera.BoundingRectangle.Y + Camera.camera.BoundingRectangle.Height - 50, 50,50), Color.Red);

            batch.End();

        }

        /*
        public void Draw2(Camera2D camera, SpriteBatch batch)
        {


            Dictionary<Tile, List<Vector2>> tileTemplate;
            Tile tile;
            Vector2 pos = new Vector2();

            // Get Tile Bounds
            Rectangle tileViewBounds = new Rectangle();

            tileViewBounds.X = (v_CameraViewBox.X / TileManager.dimensions);
            tileViewBounds.Y = (v_CameraViewBox.Y / TileManager.dimensions);
            tileViewBounds.Width = (v_CameraViewBox.Width / TileManager.dimensions) + 2;
            tileViewBounds.Height = (v_CameraViewBox.Height / TileManager.dimensions) + 2;


            TilesDrawnCount = 0;

            batch.Begin(transformMatrix: Camera.camera.Transform);


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

                        // Draw the tile
                        tile.draw(batch);

                        // Count the tile
                        TilesDrawnCount++;

                    }
                }
            }

            // Draw Visible Regions Borders
            if (DrawRegionBorders)
            {
                foreach (Region region in VisibleRegions) { batch.DrawRectangle(region.box, Color.White); }
            }

            // Draw Test Red Square at Camera Bounds view  (for testing)
            //batch.DrawRectangle(new Rectangle(Camera.camera.BoundingRectangle.X + Camera.camera.BoundingRectangle.Width - 50, Camera.camera.BoundingRectangle.Y + Camera.camera.BoundingRectangle.Height - 50, 50,50), Color.Red);

            batch.End();

        }
        */


        public void Draw (Camera2D camera, SpriteBatch batch)
        {

            //Draw3(camera, batch);
            //return;

            
            batch.Begin(transformMatrix: Camera.camera.Transform);

            v_drawnTileCount = 0;

            // Go through each CachedTiles Layer
            //foreach (KeyValuePair<TileLayer, Dictionary<Tile, List<Vector2>>> pair in v_VisibleTiles)
            foreach (TileLayer tLayer in v_VisibleTiles.Keys)
            {

                // If this given Layer does NOT exist in VisibleTiles Collection for some reason
                if (!v_VisibleTiles.ContainsKey(tLayer))
                {
                    // Skip Drawing Layer and report Error
                    Utility.Util.myDebug(true, "Map.cs Draw_VisibleMapTileCache()", "(" + tLayer.name + ") Layer key does not exist in Diciontary (v_VisibleTiles). This should never happen!");
                    continue;
                }


                // ------------------------------------------------------------------
                // DRAW EDGES - By Cached Edge Graphics
                // ------------------------------------------------------------------
                // If this Layer is the Decorations Layer
                //if (tLayer.index == 3)
                if (tLayer.name == "decorations")
                {
                    // Go through all Visible Tile Edges to draw
                    foreach (KeyValuePair<Graphic, List<Vector2>> pair2 in v_VisibleEdges)
                    {
                        // Draw ALL matching Visible Edges at once
                        pair2.Key.draw_Tiles(batch, pair2.Value, false);

                        v_drawnTileCount += pair2.Value.Count;  // Count drawn Tiles
                    }
                }


                // ------------------------------------------------------------------
                // DRAW TILES - By Cached Tile Parent (Template)
                // ------------------------------------------------------------------
                // Get Template Tile's Cached Visible Map Tile List of Locations to draw to
                Dictionary<Tile, List<Vector2>> templateList = v_VisibleTiles[tLayer];

                // Go through each Tile Template in the Layer
                foreach (KeyValuePair<Tile, List<Vector2>> pair2 in templateList)
                {
                    pair2.Key.graphic.draw_Tiles(batch, pair2.Value, false);

                    v_drawnTileCount += pair2.Value.Count;  // Count drawn Tiles
                }
            }


            // Draw Visible Regions Borders
            if (DrawRegionBorders)
            {
                foreach (Region region in VisibleRegions) { batch.DrawRectangle(region.box, Color.White); } 
            }

            // Draw Test Red Square at Camera Bounds view  (for testing)
            //batch.DrawRectangle(new Rectangle(Camera.camera.BoundingRectangle.X + Camera.camera.BoundingRectangle.Width - 50, Camera.camera.BoundingRectangle.Y + Camera.camera.BoundingRectangle.Height - 50, 50,50), Color.Red);

            batch.End();
        }


        public void Draw_VisibleMapTileCache_SpeedTest(Camera2D camera, SpriteBatch batch, int mIterationsCount)
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
            Utility.Util.myDebug("Map.cs Draw()", "Draw Tiles CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());
        }


        public void Clear()
        {

            // Clear Tile Caches
            this.VisibleRegions.Clear();
            this.v_TileTemplates.Clear();
            this.v_VisibleTiles.Clear();
            this.v_VisibleEdges.Clear();
            this.v_TileEdges.Clear();

            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }


        public void Clear_ChildTiles ()
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
            foreach (List<Vector2> template in v_VisibleEdges.Values) { template.Clear(); }
        }

    }
}
