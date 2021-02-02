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


        public virtual void draw(SpriteBatch spriteBatch, Vector2 location)
        {
            draw(spriteBatch, location, Color.White);
        }

        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, Color tintColor)
        {
            spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
        }
        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, float rotation, Vector2 origin, float scale = 1, float z = 1)
        {
            spriteBatch.Draw(texture, location, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }
        public virtual void draw(SpriteBatch spriteBatch, Vector2 location, Color tintColor, float rotation, Vector2 origin, float scale = 1, float z = 1)
        {
            spriteBatch.Draw(texture, location, null, tintColor, rotation, origin, scale, SpriteEffects.None, 0f);
        }


        public virtual void draw_Tiles(SpriteBatch spriteBatch, List<Vector2> locations)
        {
            draw_Tiles(spriteBatch, locations, Color.White);
        }


        public virtual void draw_Tiles(SpriteBatch spriteBatch, List<Vector2> locations, Color tintColor)
        {
            
            foreach (Vector2 location in locations)
            {
                spriteBatch.Draw(texture, location, null, tintColor, 0f, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 0f);
            }

        }


    }
}
