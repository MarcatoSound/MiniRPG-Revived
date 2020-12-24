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

namespace BasicRPGTest_Mono
{
    public class Main : Game
    {
        private readonly ScreenManager screenManager;
        public GraphicsDeviceManager _graphics;

        private KeyboardListener keyboardListener;

        private GameScreen activeScreen;

        public Main()
        {
            keyboardListener = new KeyboardListener();
            Components.Add(new InputListenerComponent(this, keyboardListener));
            keyboardListener.KeyPressed += (sender, args) =>
            {
                if (activeScreen is ScreenMainMenu)
                {
                    if (args.Key == Keys.Space) startGame();
                }

                if (activeScreen is ScreenGame)
                {
                    if (args.Key == Keys.Escape) Exit();
                }
            };

            screenManager = new ScreenManager();
            Components.Add(screenManager);

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            Core.camera = new OrthographicCamera(viewportadapter);

            base.Initialize();

            mainMenu();
        }


        private void mainMenu()
        {
            activeScreen = new ScreenMainMenu(this);
            screenManager.LoadScreen(activeScreen, new FadeTransition(_graphics.GraphicsDevice, Color.Black));
        }
        private void startGame()
        {
            activeScreen = new ScreenGame(this);
            screenManager.LoadScreen(activeScreen, new FadeTransition(_graphics.GraphicsDevice, Color.Black));
        }

    }
}
