using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class ItemSlot
    {
        public Item item;
        public Rectangle box;

        public ItemSlot(Item item, Rectangle box)
        {
            this.item = item;
            this.box = box;
        }


        public void Draw(SpriteBatch batch)
        {

            if (item == null) return;
            float scale = (float)box.Width / item.graphic.texture.Width;
            item.graphic.draw(batch, new Vector2(box.X, box.Y), 0f, Vector2.Zero, scale, false);
            batch.DrawRectangle(box, Color.White);

        }

    }
}
