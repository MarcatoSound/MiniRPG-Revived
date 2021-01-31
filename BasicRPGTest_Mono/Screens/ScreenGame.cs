using System;
using System.Collections.Generic;
using System.IO;
using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.GUI.HUD;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using RPGEngine;
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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private FrameCounter _frameCounter = new FrameCounter();

        private Rectangle cameraRectangle = new Rectangle();

        private bool v_NotNewDraw;

        private Texture2D cloudOverlay;


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
            Core.dmgFont = Content.Load<SpriteFont>("dmg");
            Core.critFont = Content.Load<SpriteFont>("crit");

            Game.renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);



            /*
            // Load the general game objects
            loadTiles();
            loadBiomes();
            loadItems();
            loadEntities();
            loadGuis();
            loadHud();

            if (loadMap())
                Save.save(MapManager.activeMap, worldName);*/

            loadHud();

            Texture2D texture;
            cloudOverlay = Content.Load<Texture2D>("cloud_overlay");

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
            TileManager.add(new Tile("grass", Util.getSpriteFromSet(tileset, new Rectangle(160, 0, 96, 96)), false, false, 2));
            TileManager.add(new Tile("swamp_grass", Util.getSpriteFromSet(tileset, new Rectangle(160, 96, 96, 96)), false, false, 1));
            TileManager.add(new Tile("dirt", Util.getSpriteFromSet(tileset, 0, 1), false, false, 0));
            TileManager.add(new Tile("stone", Util.getSpriteFromSet(tileset, new Rectangle(0, 160, 96, 96)), false, false, 0));
            TileManager.add(new Tile("sand", Util.getSpriteFromSet(tileset, new Rectangle(0, 64, 96, 96)), false, false, 0));
            TileManager.add(new Tile("hardened_sand", Util.getSpriteFromSet(tileset, 1, 3), false, false, 0));
            TileManager.add(new Tile("tree", Util.getSpriteFromSet(tileset, 3, 4), true, false, 5));
            TileManager.add(new Tile("water", Util.getSpriteFromSet(tileset, 0, 4), true, false));
        }

        private void loadBiomes()
        {
            Biome biome;
            biome = new Biome("field", TileManager.getByName("grass"));
            biome.undergroundTile = TileManager.getByName("dirt");
            biome.coastTile = TileManager.getByName("sand");
            biome.decorations.Add(new Engine.Maps.Generation.Decoration("bush", 5, TileManager.getByName("tree")));
            biome.decoChance = 5;
            BiomeManager.add(biome);

            biome = new Biome("desert", TileManager.getByName("sand"));
            biome.undergroundTile = TileManager.getByName("hardened_sand");
            biome.coastTile = TileManager.getByName("sand");
            BiomeManager.add(biome);

            biome = new Biome("swamp", TileManager.getByName("swamp_grass"));
            biome.undergroundTile = TileManager.getByName("dirt");
            biome.coastTile = TileManager.getByName("sand");
            biome.decorations.Add(new Engine.Maps.Generation.Decoration("bush", 1, TileManager.getByName("tree")));
            BiomeManager.add(biome);
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

            foreach (LivingEntity entity in EntityManager.livingEntities.Values)
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

            if (Directory.Exists(path))
            {
                // Load map data
                MapManager.add(new Map("overworld", size, Load.loadMap(worldName, "overworld")));


                return false;
            }


            // Generate the actual map contents
            CodeTimer codeTimer = new CodeTimer();
            codeTimer.startTimer();

            MapManager.add(new Map("overworld", size, Generator.generateOverworld(size)));

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to generate world.");


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
                // Update Map's Visible Regions
                //MapManager.activeMap.updateVisibleRegions(Camera.camera);
                MapManager.activeMap.update_VisibleRegions(Camera.camera);
            }

        }

        public override void Draw(GameTime gameTime)
        {

            // Start Code Timer. Can be used for testing different sections of the code
            Engine.Utility.CodeTimer codeTimer = new Engine.Utility.CodeTimer();
            codeTimer.startTimer();


            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            //GraphicsDevice.SetRenderTarget(Game.renderTarget);


            if (!v_NotNewDraw)
            {
                // Just in case this doesn't happen automatically. This ensures it happens right away.
                // Update Map's Visible Regions
                //MapManager.activeMap.updateVisibleRegions(Camera.camera);
                MapManager.activeMap.update_VisibleRegions(Camera.camera);
                v_NotNewDraw = true;
            }

            //MapManager.activeMap.Draw_OLD(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.Draw_SpeedTest_OLD(Camera.camera, _spriteBatch, 1000);

            //MapManager.activeMap.Draw(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.Draw_SpeedTest(Camera.camera, _spriteBatch, 1000);

            MapManager.activeMap.DrawVisibleMapCache(Camera.camera, _spriteBatch);
            // Below function is for hard speed testing function
            //MapManager.activeMap.DrawVisibleMapTileCache_SpeedTest(Camera.camera, _spriteBatch, 1000);

            _spriteBatch.Begin(transformMatrix: Camera.camera.Transform);
            _spriteBatch.Draw(cloudOverlay, Vector2.Zero, null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 16, SpriteEffects.None, 0);


            //return;  // Stop Function Here (for testing)


            List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities.Values);
            foreach (LivingEntity entity in entities)
            {
                entity.draw(_spriteBatch);
            }


            player.draw(_spriteBatch);

            _spriteBatch.End();


            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            HudManager.Draw(_spriteBatch);

            if (GuiWindowManager.activeWindow != null)
            {
                GuiWindowManager.activeWindow.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            // Draw active popup texts
            _spriteBatch.Begin(transformMatrix: Camera.camera.Transform, blendState: BlendState.NonPremultiplied);
            List<PopupText> popups = new List<PopupText>(Core.popupTexts);
            foreach (PopupText popup in popups)
            {
                if (popup == null) continue;
                popup.draw(_spriteBatch);
            }
            _spriteBatch.End();


            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(Core.dmgFont, fps, new Vector2(25, 25), Microsoft.Xna.Framework.Color.Black);
            _spriteBatch.End();

            // End Code Timer for speed test
            codeTimer.endTimer();
            // Report function's speed
            //Util.myDebug("Map.cs Draw()", "CODE TIMER:  " + codeTimer.getTotalTimeInMilliseconds());


        }



    }
}
