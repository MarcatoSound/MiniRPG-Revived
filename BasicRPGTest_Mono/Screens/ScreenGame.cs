using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.Entities;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.GUI.HUD;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using RPGEngine;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace BasicRPGTest_Mono
{
    public class ScreenGame : GameScreen
    {
        //====================================================================================
        // VARIABLES
        //====================================================================================

        private new Main Game => (Main)base.Game;

        public Player player;
        string worldName;

        private SpriteFont font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private FrameCounter _frameCounter = new FrameCounter();

        private Rectangle cameraRectangle = new Rectangle();

        private bool v_NotNewDraw;


        //====================================================================================
        // CONSTRUCTORS
        //====================================================================================

        public ScreenGame(Main game, string worldName) : base(game)
        {
            this.worldName = worldName;
        }


        //====================================================================================
        // FUNCTIONS
        //====================================================================================
        public override void LoadContent()
        {
            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Core.graphics = _graphics.GraphicsDevice;

            Game.renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);

            font = Content.Load<SpriteFont>("arial");



            // Load the general game objects
            loadTiles();
            loadBiomes();
            loadItems();
            loadEntities();
            loadGuis();
            loadHud();

            if (loadMap())
                Save.save(MapManager.activeMap, worldName);


            Texture2D texture;

            texture = Content.Load<Texture2D>("player_spriteset");
            player = new Player(texture, _graphics);

            if (loadPlayer())
                Save.save(player, worldName);
            Camera.camera.Position = player.Position;
            Camera.camera.CameraLimits = new Rectangle(0, 0, MapManager.activeMap.widthInPixels, MapManager.activeMap.heightInPixels);


            base.LoadContent();
            System.Diagnostics.Debug.WriteLine("## Loaded game content!");

            GC.Collect();
        }


        //====================================================================================
        // FUNCTIONS - LOAD / UNLOAD Content
        //====================================================================================

        public override void UnloadContent()
        {
            Save.save(MapManager.activeMap, worldName);
            Save.save(player, worldName);
            Camera.reset();
            GuiWindowManager.closeWindow();
            GuiWindowManager.Clear();
            HudManager.Clear();
            EntityManager.clear();
            TileManager.Clear();

            worldName = "";
            player = null;
            MapManager.clear();

            base.UnloadContent();

            GC.Collect();
        }


        private void loadTiles()
        {
            Texture2D tileset = Content.Load<Texture2D>("tileset_primary");
            Tile tileTemplate;

            //TiledMapTileset testTileSet = Content.Load<TiledMapTileset>("tileset_primary");
            

            int tileSize = TileManager.dimensions;

            //Graphic graphic = new Graphic(tileset);

            

            tileTemplate = new Tile("grass", Util.getSpriteFromSet(tileset, new Rectangle(160, 0, 96, 96)), false, false);
            tileTemplate.tileSetTexture = tileset;
            tileTemplate.textureRect = new Rectangle(0, 0, tileSize, tileSize);
            TileManager.add(tileTemplate);

            tileTemplate = new Tile("dirt", Util.getSpriteFromSet(tileset, 0, 1), false, false, 0);
            tileTemplate.tileSetTexture = tileset;
            tileTemplate.textureRect = new Rectangle(tileSize, 0, tileSize, tileSize);
            TileManager.add(tileTemplate);

            tileTemplate = new Tile("stone", Util.getSpriteFromSet(tileset, new Rectangle(0, 160, 96, 96)), false, false, 0);
            tileTemplate.tileSetTexture = tileset;
            tileTemplate.textureRect = new Rectangle(tileSize * 2, 0, tileSize, tileSize);
            TileManager.add(tileTemplate);

            tileTemplate = new Tile("sand", Util.getSpriteFromSet(tileset, new Rectangle(0, 64, 96, 96)), false, false, 0);
            tileTemplate.tileSetTexture = tileset;
            tileTemplate.textureRect = new Rectangle(tileSize * 3, 0, tileSize, tileSize);
            TileManager.add(tileTemplate);

            tileTemplate = new Tile("tree", Util.getSpriteFromSet(tileset, 1, 0), true, false, 2);
            tileTemplate.tileSetTexture = tileset;
            tileTemplate.textureRect = new Rectangle(tileSize * 2, tileSize, tileSize, tileSize);
            TileManager.add(tileTemplate);

            tileTemplate = new Tile("water", Util.getSpriteFromSet(tileset, 0, 4), true, false);
            tileTemplate.tileSetTexture = tileset;
            tileTemplate.textureRect = new Rectangle(tileSize * 4, 0, tileSize, tileSize);
            TileManager.add(tileTemplate);


            /*
            TileManager.add(new Tile("grass", Util.getSpriteFromSet(tileset, new Rectangle(160, 0, 96, 96)), false, false));
            TileManager.add(new Tile("dirt", Util.getSpriteFromSet(tileset, 0, 1), false, false, 0));
            TileManager.add(new Tile("stone", Util.getSpriteFromSet(tileset, new Rectangle(0, 160, 96, 96)), false, false, 0));
            TileManager.add(new Tile("sand", Util.getSpriteFromSet(tileset, new Rectangle(0, 64, 96, 96)), false, false, 0));
            TileManager.add(new Tile("tree", Util.getSpriteFromSet(tileset, 1, 0), true, false, 2));
            TileManager.add(new Tile("water", Util.getSpriteFromSet(tileset, 0, 4), true, false));
            */

        }

        private void loadBiomes()
        {
            BiomeManager.add(new Biome("ocean", TileManager.getByName("water"), null));
            BiomeManager.add(new Biome("field", TileManager.getByName("grass"), TileManager.getByName("stone")));
        }

        private void loadItems()
        {
            Texture2D sprite;
            sprite = Content.Load<Texture2D>("hollysong");
            ItemManager.add(new Tool("Hollysong", sprite, new Rectangle(0, 0, 48, 32), 10, swingDist: 1.57f));
            sprite = Content.Load<Texture2D>("arctic_fox_tail");
            ItemManager.add(new Item("Arctic Fox Tail", sprite));
            sprite = Content.Load<Texture2D>("unicorn_horn");
            ItemManager.add(new Item("Unicorn Horn", sprite));
            sprite = Content.Load<Texture2D>("sun_feather");
            ItemManager.add(new Item("Sun Feather", sprite));
            sprite = Content.Load<Texture2D>("cryorose");
            ItemManager.add(new Item("Cryorose", sprite));
            sprite = Content.Load<Texture2D>("iron_root");
            ItemManager.add(new Item("Iron Root", sprite));
        }

        private void loadEntities()
        {
            Texture2D texture = Content.Load<Texture2D>("enemy1");
            EntityManager.add(new LivingEntity("enemy1", texture, new Rectangle(0, 0, 28, 26), _graphics));
            texture = Content.Load<Texture2D>("enemy2");
            EntityManager.add(new LivingEntity("enemy2", texture, new Rectangle(0, 0, 28, 26), _graphics));
            texture = Content.Load<Texture2D>("enemy3");
            EntityManager.add(new LivingEntity("enemy3", texture, new Rectangle(0, 0, 28, 26), _graphics));

            foreach (LivingEntity entity in EntityManager.livingEntities)
            {
                System.Diagnostics.Debug.WriteLine("Entity: " + entity.name);
            }
        }

        private void loadGuis()
        {
            Texture2D texture = Content.Load<Texture2D>("gui_tileset");
            GuiWindowManager.tileset = texture;
            GuiWindowManager.add(new GuiPlayerInventory());
        }

        private void loadHud()
        {
            Texture2D texture = Content.Load<Texture2D>("hud_tileset");
            HudManager.tileset = texture;

            HotbarPrimary hotbar1 = new HotbarPrimary();
            HudManager.add(hotbar1);
            Vector2 hotbar2Pos = new Vector2(hotbar1.screenPos.X, hotbar1.screenPos.Y - 40);
            HudManager.add(new HotbarSecondary(hotbar2Pos));
        }
        private bool loadMap()
        {
            string path = $"save\\{worldName}";

            int size = 384;

            Map map;

            if (Directory.Exists(path))
            {
                map = new Map(worldName, "overworld", size, size);

                // Load map data
                //MapManager.add(new Map("overworld", size, Load.loadMap(worldName, "overworld")));
                MapManager.add(map);

                return false;
            }

            
            // Generate a new Map and Layers
            map = new Map(worldName, "overworld", size, size);
            Generator.generateOverworldTiles(map);

            // Generate the actual map contents
            //MapManager.add(new Map("overworld", size, Generator.generateOverworldTiles(size)));
            MapManager.add(map);

            return true;

        }
        private bool loadPlayer()
        {

            string path = $"save\\{worldName}";

            if (Directory.Exists(path))
            {
                
                if (!File.Exists($"{path}\\player.json"))
                    return true;
                // Load player data
                Dictionary<string, Object> playerData = Load.loadPlayer(worldName);
                player.Position = (Vector2)playerData["position"];

                return false;
            }

            return true;
        }


        //====================================================================================
        // FUNCTIONS - UPDATE & DRAW
        //====================================================================================

        public override void Update(GameTime gameTime)
        {

            //return;  // Stop Function Here (for testing)
            

            if (MapManager.activeMap == null) return;
            
            if (!Core.paused)
            {
                List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities.Values);
                foreach (LivingEntity entity in entities)
                {
                    entity.update();
                }

                player.update();
            }

            HudManager.Update();


            Camera.camera.Update(gameTime);


            // If Camera position has changed
            if (Camera.camera.BoundingRectangle != cameraRectangle)
            {
                //Engine.Utility.Util.myDebug("Camera Changed!");

                cameraRectangle = Camera.camera.BoundingRectangle;


                //Engine.Utility.CodeTimer codeTimer = new Engine.Utility.CodeTimer();
                //codeTimer.startTimer();

                //for (int i = 0; i < 1000; i++)
                //{
                    // Update Map's Visible Regions
                    MapManager.activeMap.update_VisibleRegions(Camera.camera);
                //}

                // End Code Timer for speed test
                //codeTimer.endTimer();
                // Report function's speed
                //Engine.Utility.Util.myDebug("ScreenGame.cs Update()", "Tile Cache Update CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());

            }

        }

        public override void Draw(GameTime gameTime)
        {

            // Start Code Timer. Can be used for testing different sections of the code
            //Engine.Utility.CodeTimer codeTimer = new Engine.Utility.CodeTimer();
            //codeTimer.startTimer();


            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            //GraphicsDevice.SetRenderTarget(Game.renderTarget);


            this.Draw_Map();

            //return;  // Stop Function Here (for testing)

            this.Draw_Entities();

            //return;  // Stop Function Here (for testing)

            this.Draw_HUD(gameTime);



            // End Code Timer for speed test
            //codeTimer.endTimer();
            // Report function's speed
            //Engine.Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());


        }


        // Draw Visible Map Tiles (including connected tile edges)
        private void Draw_Map ()
        {
            if (!v_NotNewDraw)
            {
                // Just in case this doesn't happen automatically. This ensures it happens right away.
                // Update Map's Visible Regions
                MapManager.activeMap.update_VisibleRegions(Camera.camera);
                v_NotNewDraw = true;
            }


            MapManager.activeMap.Draw(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.Draw_VisibleMapTileCache_SpeedTest(Camera.camera, _spriteBatch, 1000);
        }


        // Draw all Entities onto Map
        private void Draw_Entities ()
        {
            // Draw Entities
            List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities.Values);
            foreach (LivingEntity entity in entities)
            {
                // Draw Entity Sprite
                entity.draw(_spriteBatch);
                //_spriteBatch.Begin(transformMatrix: Camera.camera.Transform);
                // Draw Entity Bounding Box
                //_spriteBatch.DrawRectangle(entity.boundingBox, Microsoft.Xna.Framework.Color.White);
                //_spriteBatch.End();
            }

            // Draw Player Entity
            player.draw(_spriteBatch);
        }


        // Draw HUD and GUI
        private void Draw_HUD (GameTime gameTime)
        {
            HudManager.Draw(_spriteBatch);

            if (GuiWindowManager.activeWindow != null)
            {
                GuiWindowManager.activeWindow.Draw(_spriteBatch);
            }


            // Set up FPS string
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            // Draw FPS
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.DrawString(font, fps, new Vector2(25, 25), Microsoft.Xna.Framework.Color.Black);
            // Show count of Tiles Drawn
            _spriteBatch.DrawString(font, "Tiles Drawn: " + MapManager.activeMap.TilesDrawnCount.ToString(), new Vector2(25, 50), Microsoft.Xna.Framework.Color.Black);

            _spriteBatch.End();


        }

    }
}
