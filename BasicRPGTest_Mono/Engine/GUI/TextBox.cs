using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class TextBox
    {
        public string fullText;
        public List<string> lines;
        public Rectangle box;
        public List<RenderTarget2D> textPages;
        public int currentPage = 0;

        public float lineHeight;
        public float characterWidth;
        public int maxDisplayLines;
        public int charsPerLine;

        public TextBox(string text, Rectangle box)
        {
            Vector2 lineSize = Core.mainFont.MeasureString("W");
            lineHeight = lineSize.Y;
            characterWidth = lineSize.X;
            maxDisplayLines = (int)(box.Height / lineHeight);
            charsPerLine = (int)(box.Width / characterWidth);

            fullText = text;
            lines = new List<string>();
            this.box = box;
            textPages = new List<RenderTarget2D>();

            // Build the string list, breaking them up by words and characters
            string[] words = fullText.Split(" ");

            StringBuilder line = new StringBuilder();
            StringBuilder newLine = new StringBuilder();
            foreach (string word in words)
            {
                newLine.Append(word);

                if (newLine.ToString().Length > charsPerLine)
                {
                    lines.Add(line.ToString());
                    line.Clear();

                    newLine.Clear();
                    newLine.Append(word);
                    newLine.Append(" ");

                    line.Append(word);
                    line.Append(" ");

                    continue;
                }

                newLine.Append(" ");

                line.Append(word);
                line.Append(" ");
            }
            lines.Add(line.ToString());


            // Build the page textures based on the number of lines
            int pageCount = Math.Abs(lines.Count / maxDisplayLines) + 1;
            int lineNumber = 0;
            RenderTarget2D texture;
            for (int i = 0; i < pageCount; i++)
            {
                texture = new RenderTarget2D(Core.graphics, box.Width, box.Height);

                Core.graphics.SetRenderTarget(texture);
                Core.graphics.Clear(Color.Transparent);
                SpriteBatch batch = new SpriteBatch(Core.graphics);

                Vector2 stringPos = new Vector2();
                batch.Begin();
                for (int z = 0; z < maxDisplayLines; z++)
                {
                    if (lineNumber > lines.Count - 1) break;
                    string lineStr = lines[lineNumber];

                    stringPos.Y = lineHeight * z;
                    batch.DrawString(Core.mainFont, lineStr, stringPos, Color.SaddleBrown);

                    lineNumber++;
                }
                batch.End();
                Core.graphics.Reset();

                textPages.Add(texture);

            }

        }

        public void Draw(SpriteBatch batch)
        {

            batch.Begin();

            batch.Draw(textPages[currentPage], new Vector2(box.X, box.Y), Color.White);

            /*string line;
            Vector2 stringPos = new Vector2(box.X, box.Y);
            for (int i = 0; i < lines.Count; i++)
            {
                line = lines[i];

                stringPos.Y = box.Y + (lineHeight * i);
                batch.DrawString(Core.mainFont, line, stringPos, Color.SaddleBrown);

            }*/

            //batch.DrawRectangle(box, Color.Black);

            batch.End();
        }

    }
}
