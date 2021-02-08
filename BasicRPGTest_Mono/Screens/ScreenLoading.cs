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

            // Load the general game objects
            loadItems();
            loadTiles();
            loadBiomes();
            loadEntities();
            loadDataPacks();
            loadGuis();

            loading = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                //loadHud();

                if (loadMap())
                    Save.save(MapManager.activeMap, worldName);
            });
            loading.Start();

        }


        private void loadDataPacks()
        {
            string packsFolder = $"packs\\";

            if (!Directory.Exists(packsFolder))
                setupVanillaPack(packsFolder);

            string[] packPaths = Directory.GetDirectories(packsFolder);

            DataPack pack;
            foreach (string packPath in packPaths)
            {
                pack = new DataPack(packPath);
                pack.loadItems();
                pack.loadDropTables();
                pack.loadEntities();
            }
        }
        private void setupVanillaPack(string path)
        {
            Directory.CreateDirectory($"{path}\\");
            return;

            System.Diagnostics.Debug.WriteLine("// SETTING UP DEFAULT DATA //");
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
        }
        private void loadTiles()
        {
            TileManager.breakTexture = Content.Load<Texture2D>("breaking");
            Texture2D tileset = Content.Load<Texture2D>("tileset_primary");

            Tile tile = new Tile("grass", Util.getSpriteFromSet(tileset, new Rectangle(160, 0, 96, 96)), false, false, 2);
            TileManager.add(tile);

            tile = new Tile("swamp_grass", Util.getSpriteFromSet(tileset, new Rectangle(160, 96, 96, 96)), false, false, 1);
            TileManager.add(tile);

            tile = new Tile("dirt", Util.getSpriteFromSet(tileset, new Rectangle(256, 0, 96, 96)), false, false, 0);
            TileManager.add(tile);

            tile = new Tile("stone", Util.getSpriteFromSet(tileset, new Rectangle(0, 160, 96, 96)), true, false, 0, 50);
            TileManager.add(tile);

            tile = new Tile("sand", Util.getSpriteFromSet(tileset, new Rectangle(0, 64, 96, 96)), false, false, 0);
            TileManager.add(tile);

            tile = new Tile("hardened_sand", Util.getSpriteFromSet(tileset, 1, 3), false, false, 0);
            TileManager.add(tile);

            tile = new Tile("tree", Util.getSpriteFromSet(tileset, 3, 4), true, false, 5);
            tile.dropTable.max = 5;
            tile.dropTable.add(new ItemDrop(ItemManager.getByNamespace("ironroot"), 1, 3));
            tile.dropTable.add(new ItemDrop(ItemManager.getByNamespace("unicornhorn"), 1, 3, 0.5));
            TileManager.add(tile);

            tile = new Tile("water", Util.getSpriteFromSet(tileset, 0, 4), true, false, destructable: false);
            TileManager.add(tile);
        }

        private void loadBiomes()
        {
            Biome biome;
            Dictionary<Vector2, Tile> decoTiles = new Dictionary<Vector2, Tile>();

            biome = new Biome("field", TileManager.getByName("grass"));
            biome.undergroundTile = TileManager.getByName("dirt");
            biome.coastTile = TileManager.getByName("sand");
            biome.decorations.Add(new Decoration("bush", 60, TileManager.getByName("tree")));

            decoTiles.Add(new Vector2(0, 0), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(0, 1), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(0, 2), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(1, 0), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(1, 1), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(1, 2), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(2, 0), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(2, 1), TileManager.getByName("tree"));
            decoTiles.Add(new Vector2(2, 2), TileManager.getByName("tree"));
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
            biome.undergroundTile = TileManager.getByName("hardened_sand");
            biome.coastTile = TileManager.getByName("sand");
            BiomeManager.add(biome);


            biome = new Biome("swamp", TileManager.getByName("swamp_grass"));
            biome.undergroundTile = TileManager.getByName("dirt");
            biome.coastTile = TileManager.getByName("sand");
            biome.decorations.Add(new Decoration("bush", 30, TileManager.getByName("tree")));
            BiomeManager.add(biome);
        }

        private void loadEntities()
        {
            Texture2D texture = Content.Load<Texture2D>("enemy1");
            LivingEntity ent = new LivingEntity("purple", texture, new Rectangle(0, 0, 28, 26));
            ent.dropTable.add(new ItemDrop(ItemManager.getByNamespace("unicornhorn"), 1, 3));
            ent.dropTable.add(new ItemDrop(ItemManager.getByNamespace("ironroot")));
            EntityManager.add(ent);
            texture = Content.Load<Texture2D>("enemy2");
            ent = new LivingEntity("blue", texture, new Rectangle(0, 0, 28, 26));
            ent.dropTable.add(new ItemDrop(ItemManager.getByNamespace("unicornhorn"), 1, 3));
            ent.dropTable.add(new ItemDrop(ItemManager.getByNamespace("ironroot")));
            EntityManager.add(ent);
            texture = Content.Load<Texture2D>("enemy3");
            ent = new LivingEntity("red", texture, new Rectangle(0, 0, 28, 26));
            ent.dropTable.add(new ItemDrop(ItemManager.getByNamespace("unicornhorn"), 1, 3));
            ent.dropTable.add(new ItemDrop(ItemManager.getByNamespace("ironroot")));
            EntityManager.add(ent);

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
                progress = (Generator.mapProgress * 100).ToString("#.##");
            else
                progress = (Load.mapProgress * 100).ToString("#.##");


            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(FontLibrary.getFont("main"), $"Loading... {progress}%", new Vector2(500, 25), Color.White);
            _spriteBatch.End();
        }
    }
}
