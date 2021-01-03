using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class GuiTextBox : GuiWindow
    {
        private string text;
        public TextBox textBox;

        public GuiTextBox(string text, int padding = 10) : 
            base("textbox", new Rectangle(128, 0, 512, 128), GuiWindowManager.tileset)
        {
            this.text = text;
            Rectangle textBoxRect = new Rectangle(box.X + padding, box.Y + padding, box.Width - (padding * 2), box.Height - (padding * 2));
            textBox = new TextBox(text, textBoxRect);
        }

        public void next()
        {
            if (textBox.currentPage + 1 >= textBox.textPages.Count)
            {
                textBox.currentPage = 0;
                close();
                return;
            }
            textBox.currentPage++;
        }
        public void previous()
        {
            if (textBox.currentPage - 1 < 0) return;
            textBox.currentPage--;
        }
        public void setPage(int i)
        {
            if (i >= textBox.textPages.Count) return;
            textBox.currentPage = i;
        }


        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);

            textBox.Draw(batch);

        }

    }
}
