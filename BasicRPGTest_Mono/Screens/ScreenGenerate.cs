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
    public class ScreenGenerate : GameScreen
    {
        private new Main Game => (Main)base.Game;
        public ScreenGenerate(Main game) : base(game) { }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        string worldName;
        int cursorPos;

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
            Game.startGame(worldName);
        }

        public override void LoadContent()
        {

            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("main_font");

            worldName = "world";
            cursorPos = 0;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Vector2 textAnchor = new Vector2(font.MeasureString(worldName).X / 2, font.MeasureString(worldName).Y / 2);
            Vector2 textPos = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2 - textAnchor.X, _graphics.GraphicsDevice.Viewport.Height / 2 - textAnchor.Y);
            //System.Diagnostics.Debug.WriteLine(textAnchor);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.DrawString(font, worldName, textPos, Color.White);
            _spriteBatch.End();
        }
    }
}
