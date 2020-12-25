using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Input.InputListeners;

namespace BasicRPGTest_Mono
{
    public class ScreenMainMenu : GameScreen
    {
        private new Main Game => (Main)base.Game;
        public ScreenMainMenu(Main game) : base(game) { }


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;

        public override void LoadContent()
        {

            _graphics = Game._graphics;
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("arial");

            base.LoadContent();
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
