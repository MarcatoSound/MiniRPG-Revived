﻿using BasicRPGTest_Mono.Engine.Entities;
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
        private Dictionary<String, Dictionary<String, List<Vector2>>> v_VisibleTiles = new Dictionary<String, Dictionary<String, List<Vector2>>>();
        private Dictionary<Graphic, List<Vector2>> v_VisibleEdges = new Dictionary<Graphic, List<Vector2>>();

        private List<Tile> v_TileTemplates = new List<Tile>();
        private Dictionary<Graphic, List<Vector2>> v_EdgeTiles = new Dictionary<Graphic, List<Vector2>>();



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


            buildTileTemplateCache();
            //generateTileEdgesCache();

        }


        //====================================================================================
        // PROPERTIES
        //====================================================================================

        // Total # of Tiles on entire Map
        public long TilesTotalCount
        {
            get 
            {
                long tCount = 0;

                foreach (TileLayer tileLayer in layers)
                {
                    tCount += tileLayer.tiles.Count;
                }

                return tCount;
            }
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
                    Dictionary<String, List<Vector2>> templateList = v_VisibleTiles[tLayer.name];

                    // Go through each Tile Template in the Layer
                    foreach (String parentTileName in templateList.Keys)
                    {
                        // Get Parent Tile Template
                        Tile parentTile = TileManager.getByName(parentTileName);
                        // Get Sub-Tile Locations
                        tCount += templateList[parentTileName].Count;
                    }
                }

                return tCount;
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


        public void update_VisibleRegions (Camera2D camera)
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
            buildVisibleTileCache();

        }


        private void buildVisibleTileCache()
        {
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


            Dictionary<String, List<Vector2>> tileTemplate;

            // Create top most level of CachedTiles collection (the Map Layer Names), in order of appearance in Map.layers
            // Layer Name is first Level of multi-dimensional v_TileCache collection
            foreach (TileLayer tileLayer in layers)
            {
                tileTemplate = new Dictionary<String, List<Vector2>>();

                v_VisibleTiles.Add(tileLayer.name, tileTemplate);
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
                    tileTemplate = v_VisibleTiles[layerName];

                    // If tileTemplate Dictionary does NOT have this Tile Template Name (key) already
                    if (!tileTemplate.ContainsKey(tileParentName))
                    {
                        // Create List of Vector positions. List will contain Positions of ALL matching Tiles.
                        List<Vector2> list = new List<Vector2>();
                        // Add Position to TileCache List
                        list.Add(tile.drawPos);

                        // Add Tile template with this Tile's Position to Collection
                        tileTemplate.Add(tileParentName, list);

                    }
                    else
                    {
                        // Get Parent Tile's sub-tile position List
                        List<Vector2> list = tileTemplate[tileParentName];
                        // Add Position to TileCache List
                        list.Add(tile.drawPos);
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
                                Utility.Util.myDebug(true, "Map.cs generateTileEdgesCache()", "Missing graphic from (v_EdgeTiles)");
                            }
                        }
                    }

                }
            }

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
        }


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



        /*
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

        public void Draw_SpeedTest(Camera2D camera, SpriteBatch batch, int mIterationsCount)
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

        public void Draw_SpeedTest_OLD(Camera2D camera, SpriteBatch batch, int mIterationsCount)
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
        */

        public void Draw_VisibleMapTileCache (Camera2D camera, SpriteBatch batch)
        {
            batch.Begin(transformMatrix: Camera.camera.Transform);

            // Go through each CachedTiles Layer
            foreach (TileLayer tLayer in layers)
            {
                // Get Template Tile's Cached Visible Map Tile List of Locations to draw to
                Dictionary<String, List<Vector2>> templateList = v_VisibleTiles[tLayer.name];
                

                // If this Layer is the Decorations Layer
                if (tLayer.name == "decorations")
                {
                    // Go through all Visible Tile Edges to draw
                    foreach (KeyValuePair<Graphic, List<Vector2>> pair in v_VisibleEdges)
                    {
                        Graphic graphic = pair.Key;
                        List<Vector2> locations = pair.Value;

                        // Draw ALL matching Visible Edges at once
                        graphic.draw_Tiles(batch, locations, false);
                    }

                    /*
                    // Draw Ground Edge Tiles first (before Decorations Layer)
                    foreach (TileLayer layer in layers)
                    {
                        foreach (Region region in v_regionsVisible)
                        {
                            region.draw(batch, layer);
                        }
                    }
                    */
                }


                // Draw Tile Templates at Locations of Cached matching Map Tiles
                if (templateList != null)
                {
                    // Go through each Tile Template in the Layer
                    foreach (String parentTileName in templateList.Keys)
                    {
                        // Get Parent Tile Template
                        Tile parentTile = TileManager.getByName(parentTileName);
                        // Get Sub-Tile Locations
                        List<Vector2> list = templateList[parentTileName];

                        parentTile.graphic.draw_Tiles(batch, list, false);

                        // Count drawn Tiles
                        TilesDrawnCount += list.Count;
                    }
                
                } else
                {
                    Utility.Util.myDebug(true, "Map.cs Draw_VisibleMapTileCache()", "(" + tLayer.name + ") Layer key does not exist in Diciontary (v_TileCache). This should never happen!");
                }
            }


            // Draw Visible Regions Borders
            foreach (Region region in v_regionsVisible) { batch.DrawRectangle(region.box, Color.White); }


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
                Draw_VisibleMapTileCache(camera, batch);
            }

            // End Code Timer for speed test
            codeTimer.endTimer();
            // Report function's speed
            Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());
        }


        public void Clear()
        {

            // Clear Tile Caches
            this.v_regionsVisible.Clear();
            this.v_TileTemplates.Clear();
            this.v_VisibleTiles.Clear();
            this.v_VisibleEdges.Clear();
            this.v_EdgeTiles.Clear();

            entities.Clear();
            livingEntities.Clear();
            spawnTimer.Stop();
        }


    }
}
