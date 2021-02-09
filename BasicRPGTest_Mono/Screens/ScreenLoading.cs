using BasicRPGTest_Mono.Engine;
using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.GUI.HUD;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Maps.Generation;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace BasicRPGTest_Mono.Screens
{
    class ScreenLoading : GameScreen
    {
        private bool isGenerating;

        private new Main Game => (Main)base.Game;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private string worldName { get; set; }

        private Thread loading { get; set; }

        public ScreenLoading(Main game, string worldName, bool generate) : base(game) 
        {
            this.worldName = worldName;
            isGenerating = generate;
        }


        public override void LoadContent()
        {
            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Core.graphics = _graphics.GraphicsDevice;

            loadDataPacks();
            loadItems();
            loadTiles();

            loading = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                // Load the general game objects

                loadBiomes();
                loadGenerators();
                loadMaps();

                //if (loadMap())
                //    Save.save(MapManager.activeMap, worldName);

                loadEntities();
                loadGuis();
            });
            loading.Start();
        }


        private void loadDataPacks()
        {
            string packsFolder = $"packs\\";

            if (!Directory.Exists(packsFolder))
                setupVanillaPack(packsFolder);

            string[] packPaths = Directory.GetDirectories(packsFolder);

            foreach (string packPath in packPaths)
            {
                DataPackManager.add(new DataPack(packPath));
            }
        }
        private void setupVanillaPack(string path)
        {
            Directory.CreateDirectory($"{path}\\");
            return;

            Console.WriteLine("// SETTING UP DEFAULT DATA //");
            Directory.CreateDirectory($"{path}\\default_pack");

        }
        private void loadItems()
        {
            Texture2D sprite;
            sprite = Content.Load<Texture2D>("hollysong");
            ItemManager.add(new ParentTool("Hollysong", sprite, new Rectangle(0, 0, 48, 32), 20, swingDist: 1.57f));
            sprite = Content.Load<Texture2D>("arctic_fox_tail");
            ItemManager.add(new ParentItem("Arctic Fox Tail", sprite));
            sprite = Content.Load<Texture2D>("unicorn_horn");
            ItemManager.add(new ParentItem("Unicorn Horn", sprite, 5));
            sprite = Content.Load<Texture2D>("sun_feather");
            ItemManager.add(new ParentItem("Sun Feather", sprite));
            sprite = Content.Load<Texture2D>("cryorose");
            ItemManager.add(new ParentItem("Cryorose", sprite));
            sprite = Content.Load<Texture2D>("iron_root");
            ItemManager.add(new ParentItem("Iron Root", sprite));
            sprite = Content.Load<Texture2D>("crystal_sword");
            ParentTool pickaxe = new ParentTool("Crystal Sword", sprite, new Rectangle(), 5);
            pickaxe.damageTypes.Add(DamageType.Mining, 5);
            ItemManager.add(pickaxe);

            DataPackManager.loadItems();
        }
        private void loadTiles()
        {
            TileManager.breakTexture = Content.Load<Texture2D>("breaking");

            DataPackManager.loadTiles();
        }

        private void loadBiomes()
        {
            Biome biome;
            Dictionary<Vector2, Tile> decoTiles = new Dictionary<Vector2, Tile>();

            biome = new Biome("field", TileManager.getByName("grass"));
            biome.undergroundTile = TileManager.getByName("dirt");
            biome.coastTile = TileManager.getByName("sand");
            biome.decorations.Add(new Decoration("bush", 60, TileManager.getByName("bush")));

            decoTiles.Add(new Vector2(0, 0), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(0, 1), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(0, 2), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(1, 0), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(1, 1), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(1, 2), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(2, 0), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(2, 1), TileManager.getByName("bush"));
            decoTiles.Add(new Vector2(2, 2), TileManager.getByName("bush"));
            biome.decorations.Add(new Decoration("bush_cluster", 10, decoTiles));

            decoTiles = new Dictionary<Vector2, Tile>();
            decoTiles.Add(new Vector2(0, 0), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(0, 1), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(0, 2), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(0, 3), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(0, 4), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(1, 0), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(1, 4), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(2, 4), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(3, 0), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(3, 4), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(4, 0), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(4, 1), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(4, 2), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(4, 3), TileManager.getByName("stone"));
            decoTiles.Add(new Vector2(4, 4), TileManager.getByName("stone"));
            biome.decorations.Add(new Decoration("box", 1, decoTiles));

            biome.decoChance = 5;
            BiomeManager.add(biome);


            biome = new Biome("desert", TileManager.getByName("sand"));
            biome.undergroundTile = TileManager.getByName("sandstone");
            biome.coastTile = TileManager.getByName("sand");
            BiomeManager.add(biome);


            biome = new Biome("swamp", TileManager.getByName("swamp_grass"));
            biome.undergroundTile = TileManager.getByName("dirt");
            biome.coastTile = TileManager.getByName("sand");
            biome.decorations.Add(new Decoration("bush", 30, TileManager.getByName("bush")));
            BiomeManager.add(biome);

            DataPackManager.loadBiomes();
        }

        private void loadGenerators()
        {
            DataPackManager.loadGenerators();
        }

        private void loadMaps()
        {
            DataPackManager.loadMaps();
        }

        private void loadEntities()
        {
            DataPackManager.loadEntities();

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

            MapManager.add(new Map("overworld", size, MapGeneration.generateOverworld(size)));

            codeTimer.endTimer();
            Util.myDebug($"Took {codeTimer.getTotalTimeInMilliseconds()}ms to generate world.");


            return true;

        }

        public override void Update(GameTime gameTime)
        {

            if (!loading.IsAlive)
            {
                Game.startGame(worldName);
                return;
            }

        }
        public override void Draw(GameTime gameTime)
        {
            string progress;
            if (isGenerating)
                progress = (MapGeneration.mapProgress * 100).ToString("#.##");
            else
                progress = (Load.mapProgress * 100).ToString("#.##");


            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(FontLibrary.getFont("main"), $"Loading... {(DataPackManager.mapProgress * 100).ToString("#.##")}%", new Vector2(500, 25), Color.White);
            _spriteBatch.DrawString(FontLibrary.getFont("main"), $"Map: {DataPackManager.mapBeingLoaded}", new Vector2(500, 50), Color.White);
            _spriteBatch.End();
        }
    }
}
