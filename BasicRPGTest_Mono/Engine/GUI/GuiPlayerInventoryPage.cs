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
        private GuiPlayerInventory parent;

        private Vector2 startPos;
        private int spacing;
        private const int maxItems = 40;
        public List<Item> items;
        public List<ItemSlot> slots;

        public GuiPlayerInventoryPage(GuiPlayerInventory parent)
        {
            this.parent = parent;

            startPos = new Vector2(664, 193);
            spacing = 50;
            items = new List<Item>();
            slots = new List<ItemSlot>();


            Vector2 pos = new Vector2(startPos.X, startPos.Y);
            ItemSlot slot;
            for (int row = 1; row <= 8; row++)
            {
                for (int col = 1; col <= 5; col++)
                {
                    slot = new ItemSlot(null, new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), 24, 24));
                    slots.Add(slot);
                    pos.X += spacing;
                }
                pos.X = startPos.X;
                pos.Y += spacing;
            }
        }

        public void addItem(Item item)
        {
            if (items.Count + 1 > maxItems) return;
            items.Add(item);

            ItemSlot slot = slots[items.Count - 1];
            slot.item = item;
        }


        public void Draw(SpriteBatch batch)
        {
            foreach (ItemSlot slot in slots)
            {
                if (slot.item != null)
                {
                    slot.Draw(batch);
                }
            }
        }
    }

}
