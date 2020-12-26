using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using RPGEngine;
using System.IO;
using System.Xml.Serialization;
using BasicRPGTest_Mono.Engine;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Input.InputListeners;
using BasicRPGTest_Mono.Screens;
using BasicRPGTest_Mono.Engine.Utility;

namespace BasicRPGTest_Mono
{
    public class Main : Game
    {
        private readonly ScreenManager screenManager;
        public GraphicsDeviceManager _graphics;

        private KeyboardListener keyboardListener;

        private MouseListener mouseListener;
        private int previousScrollValue;
        private Texture2D cursorTexture;

        private GameScreen activeScreen;

        public Main()
        {

            // KEYBOARD INPUT HANDLING
            keyboardListener = new KeyboardListener();
            Components.Add(new InputListenerComponent(this, keyboardListener));
            keyboardListener.KeyPressed += (sender, args) =>
            {
                if (activeScreen is ScreenStartMenu)
                {
                    if (args.Key == Keys.Escape) Exit();
                    if (args.Key == Keys.Down) ((ScreenStartMenu)activeScreen).down();
                    if (args.Key == Keys.Up) ((ScreenStartMenu)activeScreen).up();
                    if (args.Key == Keys.Enter) ((ScreenStartMenu)activeScreen).select();
                    return;
                }

                if (activeScreen is ScreenLoadMenu)
                {
                    if (args.Key == Keys.Escape) mainMenu();
                    if (args.Key == Keys.Down) ((ScreenLoadMenu)activeScreen).down();
                    if (args.Key == Keys.Up) ((ScreenLoadMenu)activeScreen).up();
                    if (args.Key == Keys.Enter) ((ScreenLoadMenu)activeScreen).select();
                    return;
                }

                if (activeScreen is ScreenGenerate)
                {
                    if (args.Key == Keys.Escape)
                    {
                        mainMenu();
                        return;
                    }
                    if (args.Key == Keys.Back)
                        ((ScreenGenerate)activeScreen).delChar();
                    else if (args.Key == Keys.Enter)
                        ((ScreenGenerate)activeScreen).generate();
                    else
                        ((ScreenGenerate)activeScreen).enterChar(Util.getCharacter(Keyboard.GetState(), args.Key));
                    return;
                }

                if (activeScreen is ScreenGame)
                {
                    if (args.Key == Keys.Escape) mainMenu();
                    if (args.Key == Keys.LeftShift || args.Key == Keys.RightShift)
                    {
                        ((ScreenGame)activeScreen).player.Dash();
                    }

                    if (args.Key == Keys.Up) ((ScreenGame)activeScreen).player.attack(Direction.Up);
                    if (args.Key == Keys.Left) ((ScreenGame)activeScreen).player.attack(Direction.Left);
                    if (args.Key == Keys.Down) ((ScreenGame)activeScreen).player.attack(Direction.Down);
                    if (args.Key == Keys.Right) ((ScreenGame)activeScreen).player.attack(Direction.Right);

                    return;
                }


            };

            // MOUSE INPUT HANDLING
            mouseListener = new MouseListener();
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

                if (activeScreen is ScreenLoadMenu)
                {
                    if (!((ScreenLoadMenu)activeScreen).worldMenu.box.Contains(Mouse.GetState().Position)) return;
                    MouseState currentState = args.CurrentState;
                    if (currentState.ScrollWheelValue < previousScrollValue) ((ScreenLoadMenu)activeScreen).down();
                    else if (currentState.ScrollWheelValue > previousScrollValue) ((ScreenLoadMenu)activeScreen).up();
                    previousScrollValue = currentState.ScrollWheelValue;
                }
            };


            screenManager = new ScreenManager();
            Components.Add(screenManager);

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

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
                var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
                Camera.camera = new OrthographicCamera(viewportadapter);
            };

            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            Camera.camera = new OrthographicCamera(viewportadapter);

            mainMenu();
        }

        protected override void LoadContent()
        {
            cursorTexture = Content.Load<Texture2D>("cursor");
            Mouse.SetCursor(MouseCursor.FromTexture2D(cursorTexture, 0, 0));
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Core.globalTime = gameTime;
        }

        /*protected override void Draw(GameTime gameTime)
        {
            SpriteBatch batch = new SpriteBatch(_graphics.GraphicsDevice);
            batch.Begin(SpriteSortMode.BackToFront);
            batch.Draw(cursorTexture, new Vector2(Mouse.GetState().X, Mouse.GetState().Y), null, Color.White, 0f, new Vector2(), 0f, SpriteEffects.None, 1f);
            batch.End();
            base.Draw(gameTime);
        }*/

        public void mainMenu()
        {
            activeScreen = new ScreenStartMenu(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void newWorldMenu()
        {
            activeScreen = new ScreenGenerate(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void loadWorldMenu()
        {
            activeScreen = new ScreenLoadMenu(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void startGame(string worldName)
        {
            activeScreen = new ScreenGame(this, worldName);
            screenManager.LoadScreen(activeScreen, new WipeTransition(_graphics.GraphicsDevice, Color.Black, 0.7F));
        }

    }
}
