using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Input.InputListeners;
using BasicRPGTest_Mono.Engine.Menus;

namespace BasicRPGTest_Mono
{
    public class ScreenMainMenu : GameScreen
    {
        private new Main Game => (Main)base.Game;
        public ScreenMainMenu(Main game) : base(game) { }


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        private Menu mainMenu;

        public override void LoadContent()
        {

            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("main_font");

            mainMenu = new Menu("mainmenu", new Rectangle(200, 120, 400, 240), Color.Gray, Color.White, font);
            mainMenu.add(new MenuItem("Test 1")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("Menu option 0 selected!");
                    }
            });
            mainMenu.add(new MenuItem("Test 2")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("This time menu option 1 is selected!");
                    }
            });
            mainMenu.add(new MenuItem("Long Test 3")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("This is a longer string test! Entry 2");
                    }
            });
            mainMenu.add(new MenuItem("Long Test 4")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("This is a longer string test! Entry 3");
                    }
            });
            mainMenu.add(new MenuItem("Test 5")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("Menu option 4 selected!");
                    }
            });
            mainMenu.add(new MenuItem("Test 6")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("Menu option 4 selected!");
                    }
            });
            mainMenu.add(new MenuItem("Test 7")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("Menu option 4 selected!");
                    }
            });
            mainMenu.add(new MenuItem("Test 8")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("Menu option 4 selected!");
                    }
            });
            mainMenu.add(new MenuItem("Test 9")
            {
                run = () => {
                    System.Diagnostics.Debug.WriteLine("Menu option 4 selected!");
                    }
            });

            base.LoadContent();
        }

        public void down()
        {
            mainMenu.index++;
        }
        public void up()
        {
            mainMenu.index--;
        }
        public void select()
        {
            mainMenu.select();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            mainMenu.Draw(_spriteBatch, Alignment.Center, Alignment.Top);
            _spriteBatch.End();
        }
    }
}
