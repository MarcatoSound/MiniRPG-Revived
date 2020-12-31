using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Graphic
    {
        public Texture2D texture { get; set; }
        public int height { get; set; }
        public int width { get; set; }

        public Graphic(Texture2D texture)
        {
            this.texture = texture;
        }

        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, Color tintColor)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, location, tintColor);
            spriteBatch.End();
        }
        public virtual void draw(SpriteBatch spriteBatch, Vector2 location)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(texture, location, Color.White);
            spriteBatch.End();
        }
    }
}
