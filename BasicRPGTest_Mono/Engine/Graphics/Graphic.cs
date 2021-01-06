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
            width = texture.Width;
            height = texture.Height;
        }
        public Graphic()
        {

        }

        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, bool newBatch = true)
        {
            draw(spriteBatch, location, Color.White, newBatch);
        }
        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, Color tintColor, bool newBatch = true)
        {
            if (newBatch)
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);

            spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);

            if (newBatch)
                spriteBatch.End();
        }
        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, float rotation, Vector2 origin, float scale = 1, bool newBatch = true)
        {
            if (newBatch)
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);

            spriteBatch.Draw(texture, location, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);

            if (newBatch)
                spriteBatch.End();
        }
    }
}
