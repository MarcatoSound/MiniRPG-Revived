﻿using Microsoft.Xna.Framework;
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

        private GameScreen activeScreen;

        public Main()
        {
            keyboardListener = new KeyboardListener();
            Components.Add(new InputListenerComponent(this, keyboardListener));
            keyboardListener.KeyPressed += (sender, args) =>
            {
                if (activeScreen is ScreenMainMenu)
                {
                    if (args.Key == Keys.Space) worldMenu();
                    if (args.Key == Keys.Escape) Exit();
                    if (args.Key == Keys.Down) ((ScreenMainMenu)activeScreen).down();
                    if (args.Key == Keys.Up) ((ScreenMainMenu)activeScreen).up();
                    if (args.Key == Keys.Enter) ((ScreenMainMenu)activeScreen).select();
                    return;
                }

                if (activeScreen is ScreenGenerate)
                {
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
                    return;
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
            Camera.camera = new OrthographicCamera(viewportadapter);

            base.Initialize();

            mainMenu();
        }


        private void mainMenu()
        {
            activeScreen = new ScreenMainMenu(this);
            screenManager.LoadScreen(activeScreen);
        }
        private void worldMenu()
        {
            activeScreen = new ScreenGenerate(this);
            screenManager.LoadScreen(activeScreen);
        }
        public void startGame(string worldName)
        {
            activeScreen = new ScreenGame(this, worldName);
            screenManager.LoadScreen(activeScreen, new WipeTransition(_graphics.GraphicsDevice, Color.Black, 0.7F));
        }

    }
}
