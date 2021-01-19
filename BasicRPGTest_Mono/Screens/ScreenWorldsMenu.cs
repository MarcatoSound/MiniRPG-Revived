using BasicRPGTest_Mono.Engine.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BasicRPGTest_Mono.Screens
{
    public class ScreenWorldsMenu : GameScreen
    {
        private new Main Game => (Main)base.Game;
        public ScreenWorldsMenu(Main game) : base(game) { }


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        public Menu worldMenu;

        public override void LoadContent()
        {

            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("main_font");

            string path = "save";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] dirs = dirInfo.GetDirectories();

            int menuX = _graphics.PreferredBackBufferWidth / 3;
            int menuY = _graphics.PreferredBackBufferHeight / 3;
            worldMenu = new Menu("mainmenu", new Rectangle(menuX, menuY, 400, 240), Color.Gray, Color.White, font);

            foreach (DirectoryInfo dir in dirs)
            {
                worldMenu.add(new MenuItem(dir.Name)
                {
                    run = () => {
                        Game.loadWorld(dir.Name);
                    }
                });
            }

            base.LoadContent();
        }
        public void down()
        {
            worldMenu.index++;
        }
        public void up()
        {
            worldMenu.index--;
        }
        public void select()
        {
            worldMenu.select();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            worldMenu.Draw(_spriteBatch, Alignment.Center, Alignment.Top);
            _spriteBatch.End();
        }
    }
}
