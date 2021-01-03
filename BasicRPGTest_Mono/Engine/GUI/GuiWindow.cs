using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class GuiWindow
    {
        public string name;
        public Rectangle box;
        public Texture2D guiTileset;
        public RenderTarget2D texture;

        public GuiWindow(string name, Rectangle box, Texture2D tileset)
        {
            this.name = name;
            this.box = box;
            guiTileset = tileset;
            buildWindow();
        }
        public GuiWindow(string name, Rectangle box)
        {
            this.name = name;
            this.box = box;
        }

        public void buildWindow()
        {
            texture = new RenderTarget2D(Core.graphics, box.Width, box.Height);

            int dimensions = guiTileset.Width / 3;

            if (box.Width / dimensions % 1 != 0)
                throw new InvalidOperationException("GUI Window dimensions must be a multiple of " + dimensions + "... value: " + box.Width / dimensions);

            int tileWidth = box.Width / dimensions;
            int tileHeight = box.Height / dimensions;

            Vector2 pos = new Vector2();
            Core.graphics.SetRenderTarget(texture);
            Core.graphics.Clear(Color.Transparent);
            SpriteBatch batch = new SpriteBatch(Core.graphics);
            //Matrix drawingMatrix = Matrix.CreateTranslation(0, 0, 0);
            //batch.Begin(transformMatrix: drawingMatrix);
            batch.Begin();

            Texture2D topLeftCornerTexture = Util.getSpriteFromSet(guiTileset, 0, 0, dimensions);
            Texture2D topCenterTexture = Util.getSpriteFromSet(guiTileset, 0, 1, dimensions);
            Texture2D topRightCornerTexture = Util.getSpriteFromSet(guiTileset, 0, 2, dimensions);

            Texture2D midLeftTexture = Util.getSpriteFromSet(guiTileset, 1, 0, dimensions);
            Texture2D midCenterTexture = Util.getSpriteFromSet(guiTileset, 1, 1, dimensions);
            Texture2D midRightTexture = Util.getSpriteFromSet(guiTileset, 1, 2, dimensions);

            Texture2D bottomLeftTexture = Util.getSpriteFromSet(guiTileset, 2, 0, dimensions);
            Texture2D bottomCenterTexture = Util.getSpriteFromSet(guiTileset, 2, 1, dimensions);
            Texture2D bottomRightTexture = Util.getSpriteFromSet(guiTileset, 2, 2, dimensions);

            for (int row = 0; row <= tileHeight-1; row++)
            {
                for (int column = 0; column <= tileWidth-1; column++)
                {
                    if (row == 0)
                    {
                        // Draw the top-left-corner tile
                        if (column == 0)
                        {
                            batch.Draw(topLeftCornerTexture, pos, null, Color.White);
                            pos.X += dimensions;
                            continue;
                        }

                        // Draw the top-right-corner tile
                        if (column == tileWidth-1)
                        {
                            batch.Draw(topRightCornerTexture, pos, null, Color.White);
                            pos.X = 0;
                            continue;
                        }

                        // Draw the top-center tiles
                        batch.Draw(topCenterTexture, pos, null, Color.White);
                        pos.X += dimensions;

                        continue;
                    }

                    if (row == tileHeight-1)
                    {
                        // Draw the bottom-left-corner tile
                        if (column == 0)
                        {
                            batch.Draw(bottomLeftTexture, pos, null, Color.White);
                            pos.X += dimensions;
                            continue;
                        }

                        // Draw the bottom-right-corner tile
                        if (column == tileWidth-1)
                        {
                            batch.Draw(bottomRightTexture, pos, null, Color.White);
                            pos.X = 0;
                            continue;
                        }

                        // Draw the bottom-center tiles
                        batch.Draw(bottomCenterTexture, pos, null, Color.White);
                        pos.X += dimensions;

                        continue;
                    }

                    // Draw the middle-left tile
                    if (column == 0)
                    {
                        batch.Draw(midLeftTexture, pos, null, Color.White);
                        pos.X += dimensions;
                        continue;
                    }

                    // Draw the middle-right tile
                    if (column == tileWidth-1)
                    {
                        batch.Draw(midRightTexture, pos, null, Color.White);
                        pos.X = 0;
                        continue;
                    }

                    // Draw the middle-center tiles
                    batch.Draw(midCenterTexture, pos, null, Color.White);
                    pos.X += dimensions;

                }

                pos.Y += dimensions;

            }

            batch.End();
            Core.graphics.Reset();
        }

        public virtual void open()
        {
            GuiWindowManager.openWindow(this);
        }
        public virtual void close()
        {
            GuiWindowManager.closeWindow();
        }


        public virtual void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(texture, new Vector2(box.X, box.Y), Color.White);
            batch.End();
        }

    }
}
