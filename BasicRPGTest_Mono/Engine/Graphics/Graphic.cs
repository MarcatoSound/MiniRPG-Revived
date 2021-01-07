using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Graphic
    {
        //====================================================================================
        // VARIABLES
        //====================================================================================

        private Texture2D v_texture;
        public int height { get; set; }
        public int width { get; set; }


        //====================================================================================
        // CONSTRUCTORS
        //====================================================================================
        public Graphic(Texture2D texture)
        {
            this.texture = texture;
            width = texture.Width;
            height = texture.Height;
        }
        public Graphic()
        {

        }


        //====================================================================================
        // PROPERTIES
        //====================================================================================
        public Texture2D texture 
        {
            get { return v_texture; }
            set {v_texture = value; }
        }


        //====================================================================================
        // FUNCTIONS
        //====================================================================================


        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, bool newBatch = true)
        {
            draw(spriteBatch, location, Color.White, newBatch);
        }

        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, Color tintColor, bool newBatch = true)
        {
            // Slightly more efficient because one less IF check
            if (!newBatch)
            {
                spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            } else
            {
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);
                spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
                spriteBatch.End();
            }

            /*
            if (newBatch)
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);

            spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, location, null, tintColor, 0f, v_Origin, 1f, SpriteEffects.None, 0f);

            if (newBatch)
                spriteBatch.End();
            */
        }
        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, float rotation, Vector2 origin, float scale = 1, bool newBatch = true, float z = 1)
        {
            // Slightly more efficient because one less IF check
            if (!newBatch)
            {
                spriteBatch.Draw(texture, location, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);
                spriteBatch.Draw(texture, location, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                spriteBatch.End();
            }

            /*
            if (newBatch)
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);

            spriteBatch.Draw(texture, location, null, Color.White, rotation, origin, scale, SpriteEffects.None, z);


            if (newBatch)
                spriteBatch.End();
            */
        }


        public virtual void draw_Tiles(SpriteBatch spriteBatch, List<Vector2> locations, bool newBatch = true)
        {
            draw_Tiles(spriteBatch, locations, Color.White, newBatch);
        }


        public virtual void draw_Tiles(SpriteBatch spriteBatch, List<Vector2> locations, Color tintColor, bool newBatch = true)
        {

            // Slightly more efficient because one less IF check
            if (!newBatch)
            {
                foreach (Vector2 location in locations)
                {
                    draw(spriteBatch, location, tintColor, false);
                    //spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
                }
            }
            else
            {
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);

                foreach (Vector2 location in locations)
                {
                    draw(spriteBatch, location, tintColor, true);
                    //spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
                }

                spriteBatch.End();
            }

            /*
            if (newBatch)
                spriteBatch.Begin(transformMatrix: Camera.camera.Transform);

            //spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, location, null, tintColor, 0f, v_Origin, 1f, SpriteEffects.None, 0f);

            if (newBatch)
                spriteBatch.End();
            */
        }


    }
}
