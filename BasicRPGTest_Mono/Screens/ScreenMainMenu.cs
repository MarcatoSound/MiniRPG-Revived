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

            font = Content.Load<SpriteFont>("arial");

            mainMenu = new Menu("mainmenu");
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(font, "Press [Space] to play!", new Vector2(25, 25), Color.Black);
            _spriteBatch.End();
        }
    }
}
