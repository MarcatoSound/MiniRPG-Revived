using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BasicRPGTest_Mono.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;

namespace BasicRPGTest_Mono
{
    public class ScreenGame : GameScreen
    {
        private new Main Game => (Main)base.Game;

        Player player;
        LivingEntity entity;
        string worldName;

        private SpriteFont font;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        TiledMapTileset masterTileset;
        TiledMapRenderer _tiledMapRenderer;

        private FrameCounter _frameCounter = new FrameCounter();
        public ScreenGame(Main game, string worldName) : base(game)
        {
            this.worldName = worldName;
        }

        public override void LoadContent()
        {
            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("arial");


            // Load the general game objects
            //loadTiles();
            loadEntities();

            // Load the map data
            buildTileset();

            generateMap();

            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, MapManager.activeMap.tiledMap);

            Texture2D texture = Content.Load<Texture2D>("test_sprite_atlas");
            player = new Player(texture, _graphics);


            base.LoadContent();
            System.Diagnostics.Debug.WriteLine("## Loaded game content!");
        }
        public override void UnloadContent()
        {
            Save.save(MapManager.activeMap.tiledMap, worldName);
            Camera.reset();

            worldName = "";
            player = null;
            MapManager.clear();

            base.UnloadContent();
        }

        private void buildTileset()
        {

            masterTileset = new TiledMapTileset(Content.Load<Texture2D>("[Base]BaseChip_pipo"), 32, 32, 96, 0, 0, 8);
            TiledMapTilesetTile tile;
            for (int i = 0; i < masterTileset.TileCount; i++)
            {
                tile = new TiledMapTilesetTile(i);
                masterTileset.Tiles.Add(tile);
            }

        }

        private void generateMap()
        {
            string path = $"save\\{worldName}";

            if (Directory.Exists(path))
            {
                MapManager.add(new Map(Load.loadMap(masterTileset, worldName)));
                return;
            }

            int size = 128;

            // Generate the actual map contents
            MapManager.add(new Map(Generator.generateOverworld(size)));
            MapManager.activeMap.tiledMap.AddTileset(masterTileset, 0);

            // Saving world functionality
            Save.save(MapManager.activeMap.tiledMap, worldName);


        }

        private void loadEntities()
        {
            Texture2D texture = Content.Load<Texture2D>("enemy1");
            EntityManager.add(new LivingEntity(texture, new Rectangle(0, 0, 28, 26), _graphics));
            texture = Content.Load<Texture2D>("enemy2");
            EntityManager.add(new LivingEntity(texture, new Rectangle(0, 0, 28, 26), _graphics));
            texture = Content.Load<Texture2D>("enemy3");
            EntityManager.add(new LivingEntity(texture, new Rectangle(0, 0, 28, 26), _graphics));
        }


        public override void Update(GameTime gameTime)
        {
            List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities);
            foreach (LivingEntity entity in entities)
            {
                entity.update();
            }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                // Logic for converting camera position and player position to player TILE position
                Vector2 playerMapPos = player.getPlayerTilePositionPrecise();
                System.Diagnostics.Debug.WriteLine("Player tile position: " + playerMapPos);
                System.Diagnostics.Debug.WriteLine("Player map position: " + player.position);
            }

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.Right))
                player.move(gameTime);


            if (kstate.IsKeyDown(Keys.LeftShift))
                player.speed = 200f;
            else player.speed = 100f;

            player.update();
            Camera.camera.Position = Camera.camPos;
            _tiledMapRenderer.Update(gameTime);

        }

        public override void Draw(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _frameCounter.Update(deltaTime);
            var fps = string.Format("FPS: {0}", _frameCounter.AverageFramesPerSecond);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.End();

            _tiledMapRenderer.Draw(Camera.camera.GetViewMatrix());

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(font, fps, new Vector2(25, 25), Color.Black);
            _spriteBatch.End();

            player.draw(_spriteBatch);

            List<LivingEntity> entities = new List<LivingEntity>(MapManager.activeMap.livingEntities);
            foreach (LivingEntity entity in entities)
            {
                entity.draw(_spriteBatch);
                _spriteBatch.Begin();
                _spriteBatch.DrawRectangle(entity.getScreenBox(), Color.White);
                _spriteBatch.End();
            }


            /*foreach (Rectangle tileBox in MapManager.activeMap.collidables)
            {
                _spriteBatch.DrawRectangle(tileBox, Color.White);
            }*/

        }

    }
}
