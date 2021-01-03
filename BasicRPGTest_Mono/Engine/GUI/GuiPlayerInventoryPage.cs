using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{
    public class GuiPlayerInventoryPage
    {
        private Vector2 startPos;
        private int spacing;
        private const int maxItems = 40;
        public List<Item> items;

        public GuiPlayerInventoryPage()
        {
            startPos = new Vector2(302, 53);
            spacing = 50;
            items = new List<Item>();
        }

        public void addItem(Item item)
        {
            if (items.Count + 1 > maxItems) return;
            items.Add(item);
        }


        public void Draw(SpriteBatch batch)
        {
            Vector2 pos = new Vector2(startPos.X, startPos.Y);
            int itemNumber = 0;
            Item item;
            float scale;

            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 5; col++)
                {
                    item = items[itemNumber];
                    if (item != null)
                    {
                        scale = 24.0f / item.graphic.texture.Width;
                        item.graphic.draw(batch, pos, 0f, Vector2.Zero, scale, false);
                    }
                    //System.Diagnostics.Debug.WriteLine("Item number: " + itemNumber);
                    itemNumber++;
                    pos.X += spacing;
                }
                pos.X = startPos.X;
                pos.Y += spacing;
            }

        }
    }

}
