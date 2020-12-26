using BasicRPGTest_Mono.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace RPGEngine
{

    public class GraphicAnimated : Graphic
    {
        public int rows { get; set; }
        public int columns { get; set; }
        public int currentFrame;
        private int totalFrames;


        public GraphicAnimated(Texture2D texture, int rows, int columns) : base (texture)
        {
            this.texture = texture;
            this.rows = rows;
            this.columns = columns;
            currentFrame = 0;
            totalFrames = this.rows * this.columns;
        }
        public GraphicAnimated(GraphicAnimated graphic) : base (graphic.texture)
        {
            texture = graphic.texture;
            rows = graphic.rows;
            columns = graphic.columns;
            currentFrame = 0;
            totalFrames = rows * columns;
        }


        public void update()
        {
            currentFrame++;
            if (currentFrame == totalFrames)
                currentFrame = 0;
        }

        public override void draw(SpriteBatch spriteBatch, Vector2 location)
        {
            width = texture.Width / columns;
            height = texture.Height / rows;
            int row = (int)((float)currentFrame / (float)columns);
            int column = currentFrame % columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Begin();
            spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, Color.White, 0f, new Vector2(width / 2, height / 2), SpriteEffects.None, 0f);
            spriteBatch.End();
        }

    }
}
