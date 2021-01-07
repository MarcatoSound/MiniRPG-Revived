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

            font = Content.Load<SpriteFont>("arial");



            // Load the general game objects
            loadTiles();
            loadBiomes();
            loadItems();
            loadEntities();
            loadGuis();
            loadHud();

            loadMap();


            Texture2D texture;

            texture = Content.Load<Texture2D>("player_spriteset");
            player = new Player(texture, _graphics);

            loadPlayer();
            Camera.camera.Position = player.Position;
            Camera.camera.CameraLimits = new Rectangle(0, 0, MapManager.activeMap.widthInPixels, MapManager.activeMap.heightInPixels);


            // Saving functionality
            Save.save(MapManager.activeMap, worldName);
            Save.save(player, worldName);


            base.LoadContent();
            System.Diagnostics.Debug.WriteLine("## Loaded game content!");
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
        }


        private void loadTiles()
        {
            Texture2D tileset = Content.Load<Texture2D>("tileset_primary");
            TileManager.add(new Tile("grass", Util.getSpriteFromSet(tileset, 0, 0), false, false));
            TileManager.add(new Tile("dirt", Util.getSpriteFromSet(tileset, 0, 1), false, false));
            TileManager.add(new Tile("stone", Util.getSpriteFromSet(tileset, 0, 2), false, false));
            TileManager.add(new Tile("sand", Util.getSpriteFromSet(tileset, 0, 3), false, false));
            TileManager.add(new Tile("tree", Util.getSpriteFromSet(tileset, 1, 0), true, false));
            TileManager.add(new Tile("water", Util.getSpriteFromSet(tileset, 0, 4), true, false));
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

        private void loadMap()
        {
            string path = $"save\\{worldName}";

            int size = 384;

            if (Directory.Exists(path))
            {
                // Load map data
                MapManager.add(new Map("overworld", size, Load.loadMap(worldName)));


                return;
            }


            // Generate the actual map contents
            MapManager.add(new Map("overworld", size, Generator.generateOverworldTiles(size)));

        }

        private void loadPlayer()
        {

            string path = $"save\\{worldName}";

            if (Directory.Exists(path))
            {
                // Load player data
                Dictionary<string, Object> playerData = Load.loadPlayer(worldName);
                player.Position = (Vector2)playerData["position"];

                return;
            }
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
                // Update Map's Visible Regions
                MapManager.activeMap.update_VisibleRegions(Camera.camera);
            }

        }

        public override void Draw(GameTime gameTime)
        {

            // Start Code Timer. Can be used for testing different sections of the code
            //Engine.Utility.CodeTimer codeTimer = new Engine.Utility.CodeTimer();
            //codeTimer.startTimer();


            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.End();


            //return;  // Stop Function Here (for testing)


            //MapManager.activeMap.Draw_OLD(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.Draw_SpeedTest_OLD(Camera.camera, _spriteBatch, 1000);

            //MapManager.activeMap.Draw(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.Draw_SpeedTest(Camera.camera, _spriteBatch, 1000);

            MapManager.activeMap.Draw_TileCache(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.Draw_TileCache_SpeedTest(Camera.camera, _spriteBatch, 1000);


            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(font, fps, new Vector2(25, 25), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.End();


            //return;  // Stop Function Here (for testing)


            List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities.Values);
            foreach (LivingEntity entity in entities)
            {
                entity.draw(_spriteBatch);
                _spriteBatch.Begin(transformMatrix: Camera.camera.Transform);
                _spriteBatch.DrawRectangle(entity.boundingBox, Microsoft.Xna.Framework.Color.White);
                _spriteBatch.End();
            }

            player.draw(_spriteBatch);

            HudManager.Draw(_spriteBatch);

            if (GuiWindowManager.activeWindow != null)
            {
                GuiWindowManager.activeWindow.Draw(_spriteBatch);
            }



            // End Code Timer for speed test
            //codeTimer.endTimer();
            // Report function's speed
            //Engine.Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());


        }


        public void Draw_SpeedTest (GameTime gameTime, int mIterationsCount)
        {
            // Start Code Timer for speed test
            Engine.Utility.CodeTimer codeTimer = new Engine.Utility.CodeTimer();
            codeTimer.startTimer();

            // If No Interation count as given, use Default of 1000
            if (mIterationsCount <= 0) { mIterationsCount = 1000; }

            for (int i = 0; i < mIterationsCount; i++)
            {
                // Draw Screen
                Draw(gameTime);
            }

            // End Code Timer for speed test
            codeTimer.endTimer();
            // Report function's speed
            Engine.Utility.Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());

        }
    }
}
