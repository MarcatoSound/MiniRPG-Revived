using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens.Transitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono
{
    public class TiledTransition : Transition
    {
        private readonly GraphicsDevice graphics;
        private readonly SpriteBatch batch;
        public Color color { get; set; }
        public TiledTransition(GraphicsDevice graphics, Color color, float duration = 1.0F) : base(duration)
        {
            this.color = color;

            this.graphics = graphics;
            this.batch = new SpriteBatch(graphics);
        }
        public override void Dispose()
        {
            batch.Dispose();
        }

        public override void Draw(GameTime gameTime)
        {
            var width = graphics.Viewport.Width;
            var height = graphics.Viewport.Height * (Value + 0.05F);
            var rect = new RectangleF(0, 0, width, height);

            batch.Begin(samplerState: SamplerState.PointClamp);
            batch.FillRectangle(rect, color);
            batch.End();

        }
    }
}
