using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using BasicRPGTest_Mono.Engine;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Input.InputListeners;
using BasicRPGTest_Mono.Screens;
using BasicRPGTest_Mono.Engine.Utility;
using BasicRPGTest_Mono.Engine.GUI;
using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.GUI.Text;
using BasicRPGTest_Mono.Engine.Entities;
using SharpNoise.Utilities.Imaging;
using System.Drawing.Imaging;
using SharpNoise.Modules;
using SharpNoise.Builders;
using SharpNoise;

namespace BasicRPGTest_Mono
{
    public class Main : Game
    {

        //====================================================================================
        // CONSOLE WINDOW HANDLING
        //====================================================================================
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private static bool showConsole = false;


        //====================================================================================
        // MAIN FIELDS
        //====================================================================================

        private readonly ScreenManager screenManager;
        public GraphicsDeviceManager _graphics;

        private KeyboardListener keyboardListener;

        private MouseListener mouseListener;
        private int previousScrollValue;
        private Texture2D cursorTexture;

        private GameScreen activeScreen;

        public RenderTarget2D renderTarget;

        public Main()
        {
            if (showConsole)
                ShowConsoleWindow();

            // KEYBOARD INPUT HANDLING
            KeyboardListenerSettings kListenerSettings = new KeyboardListenerSettings();
            kListenerSettings.RepeatPress = false;
            keyboardListener = new KeyboardListener(kListenerSettings);
            Components.Add(new InputListenerComponent(this, keyboardListener));
            keyboardListener.KeyPressed += (sender, args) =>
            {
                if (args.Key == Keys.OemTilde)
                    toggleConsole();

                if (activeScreen is ScreenStartMenu)
                {
                    if (args.Key == Keys.Escape) Exit();
                    if (args.Key == Keys.Down) ((ScreenStartMenu)activeScreen).down();
                    if (args.Key == Keys.Up) ((ScreenStartMenu)activeScreen).up();
                    if (args.Key == Keys.Enter) ((ScreenStartMenu)activeScreen).select();
                    return;
                }

                if (activeScreen is ScreenWorldsMenu)
                {
                    if (args.Key == Keys.Escape) mainMenu();
                    if (args.Key == Keys.Down) ((ScreenWorldsMenu)activeScreen).down();
                    if (args.Key == Keys.Up) ((ScreenWorldsMenu)activeScreen).up();
                    if (args.Key == Keys.Enter) ((ScreenWorldsMenu)activeScreen).select();
                    return;
                }

                if (activeScreen is ScreenNewWorld)
                {
                    if (args.Key == Keys.Escape)
                    {
                        mainMenu();
                        return;
                    }
                    if (args.Key == Keys.Back)
                        ((ScreenNewWorld)activeScreen).delChar();
                    else if (args.Key == Keys.Enter)
                        ((ScreenNewWorld)activeScreen).generate();
                    else
                        ((ScreenNewWorld)activeScreen).enterChar(Util.getCharacter(Keyboard.GetState(), args.Key));
                    return;
                }

                if (activeScreen is ScreenGame)
                {
                    if (args.Key == Keys.Space)
                    {
                        // Debug key

                        if (MapManager.activeMap.name == "overworld")
                            MapManager.activeMap = MapManager.getByName("cavern");
                        else
                            MapManager.activeMap = MapManager.getByName("overworld");

                        /*Random seedGenerator = new Random();
                        int seed = seedGenerator.Next(0, 9999999);

                        double scale = 1;
                        double persistance = 0.5;
                        int octaves = 4;

                        Perlin gen = new Perlin();
                        gen.Seed = seed;
                        gen.Frequency = scale / 66.66;
                        gen.Persistence = persistance;
                        gen.Lacunarity = 2.25;
                        gen.OctaveCount = octaves;

                        ScaleBias mod = new ScaleBias();
                        mod.Source0 = gen;
                        mod.Bias = 0.05;
                        mod.Scale = 0.68;

                        PlaneNoiseMapBuilder builder = new PlaneNoiseMapBuilder();
                        builder.SourceModule = mod;
                        builder.SetBounds(0, 512, 0, 512);
                        NoiseMap map = new NoiseMap(512, 512);
                        builder.DestNoiseMap = map;
                        builder.SetDestSize(512, 512);
                        builder.Build();

                        Image img = new Image();
                        ImageRenderer renderer = new ImageRenderer();
                        renderer.DestinationImage = img;
                        renderer.SourceNoiseMap = map;
                        renderer.BuildTerrainGradient();
                        renderer.Render();

                        img.SaveGdiBitmap("noise.png", ImageFormat.Png);*/

                    }
                    if (args.Key == Keys.Escape) mainMenu();

                    if (!Core.paused)
                    {
                        if (args.Key == Keys.LeftShift || args.Key == Keys.RightShift)
                        {
                            Core.player.Dash();
                        }

                        if (args.Key == Keys.W) Core.player.setDirection(Direction.Up);
                        if (args.Key == Keys.S) Core.player.setDirection(Direction.Down);
                        if (args.Key == Keys.A) Core.player.setDirection(Direction.Left);
                        if (args.Key == Keys.D) Core.player.setDirection(Direction.Right);

                        if (args.Key == Keys.Up) Core.player.attack(Direction.Up);
                        if (args.Key == Keys.Down) Core.player.attack(Direction.Down);
                        if (args.Key == Keys.Left) Core.player.attack(Direction.Left);
                        if (args.Key == Keys.Right) Core.player.attack(Direction.Right);

                        if (args.Key == Keys.F) Core.player.swapHotbars();
                        if (args.Key == Keys.E) Core.player.toggleInv();

                        if (args.Key == Keys.NumPad1) Core.player.inventory.hotbarPrimary.setSlot(0);
                        if (args.Key == Keys.NumPad2) Core.player.inventory.hotbarPrimary.setSlot(1);
                        if (args.Key == Keys.NumPad3) Core.player.inventory.hotbarPrimary.setSlot(2);
                        if (args.Key == Keys.NumPad4) Core.player.inventory.hotbarPrimary.setSlot(3);
                        if (args.Key == Keys.NumPad5) Core.player.inventory.hotbarPrimary.setSlot(4);
                    } else
                    {
                        if (args.Key == Keys.E) Core.player.toggleInv();
                        //if (args.Key == Keys.Down || args.Key == Keys.S) Core.player.activeMenu.index++;
                        //if (args.Key == Keys.Up || args.Key == Keys.W) Core.player.activeMenu.index--;
                        if (args.Key == Keys.Enter)
                        {
                            if (GuiWindowManager.activeWindow is GuiTextBox)
                            {
                                ((GuiTextBox)GuiWindowManager.activeWindow).next();
                            }
                        }

                        if (args.Key == Keys.Right)
                        {
                            if (GuiWindowManager.activeWindow is GuiPlayerInventory)
                            {
                                ((GuiPlayerInventory)GuiWindowManager.activeWindow).nextPage();
                            }
                        }
                        if (args.Key == Keys.Left)
                        {
                            if (GuiWindowManager.activeWindow is GuiPlayerInventory)
                            {
                                ((GuiPlayerInventory)GuiWindowManager.activeWindow).previousPage();
                            }
                        }
                    }

                    return;
                }


            };

            // MOUSE INPUT HANDLING
            MouseListenerSettings mListenerSettings = new MouseListenerSettings();
            mListenerSettings.DoubleClickMilliseconds = 50;
            mouseListener = new MouseListener(mListenerSettings);
            Components.Add(new InputListenerComponent(this, mouseListener));
            mouseListener.MouseWheelMoved += (sender, args) =>
            {
                previousScrollValue = args.PreviousState.ScrollWheelValue;
                if (activeScreen is ScreenStartMenu)
                {
                    if (!((ScreenStartMenu)activeScreen).mainMenu.box.Contains(Mouse.GetState().Position)) return;
                    MouseState currentState = args.CurrentState;
                    if (currentState.ScrollWheelValue < previousScrollValue) ((ScreenStartMenu)activeScreen).down();
                    else if (currentState.ScrollWheelValue > previousScrollValue) ((ScreenStartMenu)activeScreen).up();
                    previousScrollValue = currentState.ScrollWheelValue;
                }

                if (activeScreen is ScreenWorldsMenu)
                {
                    if (!((ScreenWorldsMenu)activeScreen).worldMenu.box.Contains(Mouse.GetState().Position)) return;
                    MouseState currentState = args.CurrentState;
                    if (currentState.ScrollWheelValue < previousScrollValue) ((ScreenWorldsMenu)activeScreen).down();
                    else if (currentState.ScrollWheelValue > previousScrollValue) ((ScreenWorldsMenu)activeScreen).up();
                    previousScrollValue = currentState.ScrollWheelValue;
                }

                if (activeScreen is ScreenGame)
                {
                    MouseState currentState = args.CurrentState;
                    if (currentState.ScrollWheelValue < previousScrollValue) Camera.camera.Scale *= 0.9f;
                    else if (currentState.ScrollWheelValue > previousScrollValue) Camera.camera.Scale *= 1.1f;
                        previousScrollValue = currentState.ScrollWheelValue;
                }

            };
            mouseListener.MouseClicked += (sender, args) =>
            {
                if (activeScreen is ScreenGame)
                {
                    if (GuiWindowManager.activeWindow is GuiPlayerInventory)
                    {
                        GuiPlayerInventory playerInv = (GuiPlayerInventory)GuiWindowManager.activeWindow;

                        if (args.Button == MonoGame.Extended.Input.MouseButton.Left)
                        {
                            if (playerInv.cursorItem == null)
                            {
                                int slotIndex;
                                Item item = playerInv.getItemAt(args.Position, out slotIndex);
                                // TODO: Prevent inventory from closing while item is in cursor
                                if (item != null)
                                {
                                    if (slotIndex > 13)
                                        slotIndex += 40 * playerInv.currentPage;
                                    playerInv.cursorItem = item;
                                    playerInv.removeItem(slotIndex);


                                    playerInv.updateGui();
                                }
                            }
                            else
                            {
                                int slotIndex;
                                ItemSlot slot = playerInv.getSlotAt(args.Position, out slotIndex);
                                if (slot != null)
                                {
                                    if (slotIndex > 13)
                                        slotIndex += 40 * playerInv.currentPage;
                                    playerInv.addItem(slotIndex, playerInv.cursorItem);
                                    //playerInv.cursorItem = null;


                                    playerInv.updateGui();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (args.Button == MonoGame.Extended.Input.MouseButton.Left)
                        {
                            Vector2 clickPos = new Vector2(args.Position.X, args.Position.Y);
                            clickPos = Util.screenPosToTruePos(clickPos);
                            new ItemEntity(MapManager.activeMap, new Item(ItemManager.getByNamespace("unicornhorn")), clickPos);
                        }
                    }
                }
            };


            screenManager = new ScreenManager();
            Components.Add(screenManager);

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Core.game = this;
            Core.graphics = _graphics.GraphicsDevice;
            Core.content = Content;

            Window.AllowUserResizing = true;
            
        }

        protected override void Initialize()
        {

            base.Initialize();

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            Window.ClientSizeChanged += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine("Width: " + _graphics.PreferredBackBufferWidth);
                System.Diagnostics.Debug.WriteLine("Height: " + _graphics.PreferredBackBufferHeight);
                System.Diagnostics.Debug.WriteLine("Client Width: " + Window.ClientBounds.Width);
                System.Diagnostics.Debug.WriteLine("Client Height: " + Window.ClientBounds.Height);
                _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
                Camera.camera.Initialize();
            };

            Camera.camera = new Camera2D(this);
            Camera.camera.Initialize();

            mainMenu();
        }

        protected override void LoadContent()
        {
            cursorTexture = Content.Load<Texture2D>("cursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursorTexture, 0, 0));

            SpriteFont font = Content.Load<SpriteFont>("font_main");
            FontLibrary.addFont("main", font);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Core.globalTime = gameTime;
        }


        public void mainMenu()
        {
            activeScreen = new ScreenStartMenu(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void newWorldMenu()
        {
            activeScreen = new ScreenNewWorld(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void loadWorldMenu()
        {
            activeScreen = new ScreenWorldsMenu(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void loadWorld(string worldName, bool generate)
        {
            activeScreen = new ScreenLoading(this, worldName, generate);
            screenManager.LoadScreen(activeScreen);
        }
        public void startGame(string worldName)
        {
            activeScreen = new ScreenGame(this, worldName);
            screenManager.LoadScreen(activeScreen, new WipeTransition(_graphics.GraphicsDevice, Microsoft.Xna.Framework.Color.Black, 0.7F));
        }



        public static void ShowConsoleWindow()
        {
            var handle = GetConsoleWindow();

            if (handle == IntPtr.Zero)
            {
                AllocConsole();
            }
            else
            {
                ShowWindow(handle, SW_SHOW);
            }
        }
        public static void HideConsoleWindow()
        {
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
        }
        public static void toggleConsole()
        {
            if (showConsole)
            {
                HideConsoleWindow();
                showConsole = false;
            }
            else
            {
                ShowConsoleWindow();
                showConsole = true;
            }
        }

    }
}
