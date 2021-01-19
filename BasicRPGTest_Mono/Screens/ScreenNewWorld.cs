using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Screens
{
    public class ScreenNewWorld : GameScreen
    {
        private new Main Game => (Main)base.Game;
        public ScreenNewWorld(Main game) : base(game) { }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        private string worldName;

        public void enterChar(char letter)
        {
            if (letter == (char)0) return;
            StringBuilder sb = new StringBuilder(worldName);
            sb.Append(letter.ToString());
            worldName = sb.ToString();
        }
        public void delChar()
        {
            StringBuilder sb = new StringBuilder(worldName);
            if (sb.Length - 1 < 0) return;
            sb.Remove(sb.Length - 1, 1);
            worldName = sb.ToString();
        }
        public void generate()
        {
            Game.loadWorld(worldName, true);
        }

        public override void LoadContent()
        {

            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("main_font");

            worldName = "world";

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Vector2 textAnchor;
            Vector2 textPos;
            //System.Diagnostics.Debug.WriteLine(textAnchor);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            textAnchor = new Vector2(font.MeasureString(worldName).X / 2, font.MeasureString(worldName).Y / 2);
            textPos = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - textAnchor.X, _graphics.GraphicsDevice.Viewport.Height / 2 - textAnchor.Y);
            _spriteBatch.DrawString(font, worldName, textPos, Color.White);

            string str = "Enter world name:";
            textAnchor = new Vector2(font.MeasureString(str).X / 2, font.MeasureString(str).Y / 2);
            textPos = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - textAnchor.X, _graphics.GraphicsDevice.Viewport.Height / 2 - textAnchor.Y - 50);
            _spriteBatch.DrawString(font, str, textPos, Color.White);
            _spriteBatch.End();
        }
    }
}
