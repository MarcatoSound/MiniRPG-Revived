using BasicRPGTest_Mono.Engine.Inventories;
using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{

    public class GuiPlayerInventory : GuiWindow
    {

        public Texture2D background;
        public Vector2 position;
        public PlayerInventory inventory;
        public List<GuiPlayerInventoryPage> pages;

        public List<ItemSlot> itemSlots;

        public Item cursorItem;

        private int _currentPage;
        public int currentPage
        {
            get { return _currentPage; }
            set
            {
                if (value > pages.Count - 1 || value < 0) return;
                _currentPage = value;
                updateGui();
            }
        }

        public GuiPlayerInventory() : base("playerinventory", new Rectangle(362, 140, 556, 452))
        {
            position = new Vector2(box.X, box.Y);
            background = Core.content.Load<Texture2D>("gui_inventory");

            pages = new List<GuiPlayerInventoryPage>();
            itemSlots = new List<ItemSlot>();
            currentPage = 0;
        }

        public void updateGui()
        {
            inventory = Core.player.inventory;
            pages.Clear();
            itemSlots.Clear();

            // Build the armor
            int slotDist = 51;

            Vector2 itemPos = new Vector2(position.X + 20, position.Y + 68); // This is the starting position for drawing the armor items.
            for (int i = 0; i < 4; i++)
            {
                // Item is null as armor is unimplemented.
                itemSlots.Add(new ItemSlot(null, new Rectangle(Convert.ToInt32(itemPos.X), Convert.ToInt32(itemPos.Y), 38, 38)));
                itemPos.Y += slotDist;
            }


            // Build the hotbars
            slotDist = 51;

            itemPos = new Vector2(position.X + 20, position.Y + 308); // This is the starting position for drawing the primary hotbar's items.
            foreach (Item item in inventory.hotbarPrimary.items.Values)
            {
                itemSlots.Add(new ItemSlot(item, new Rectangle(Convert.ToInt32(itemPos.X), Convert.ToInt32(itemPos.Y), 38, 38)));
                itemPos.X += slotDist;
            }

            itemPos = new Vector2(position.X + 20, position.Y + 388); // This is the starting position for drawing the secondary hotbar's items.
            foreach (Item item in inventory.hotbarSecondary.items.Values)
            {
                itemSlots.Add(new ItemSlot(item, new Rectangle(Convert.ToInt32(itemPos.X), Convert.ToInt32(itemPos.Y), 38, 38)));
                itemPos.X += slotDist;
            }


            // Build the item page(s)
            int pageCount = 2;
            int itemNumber = 0;
            int maxPageItems = 40;
            GuiPlayerInventoryPage page;
            for (int i = 1; i <= pageCount; i++)
            {
                page = new GuiPlayerInventoryPage(this);
                for (int z = 0; z < maxPageItems; z++)
                {
                    page.addItem(inventory.getItem(itemNumber));
                    itemNumber++;
                }
                pages.Add(page);
            }

            itemSlots.AddRange(pages[currentPage].slots);


            /*inventory = Core.player.inventory;
            pages.Clear();

            // Generate the GUI window
            if (texture != null) texture.Dispose();
            texture = new RenderTarget2D(Core.graphics, box.Width, box.Height);

            Core.graphics.SetRenderTarget(texture);
            Core.graphics.Clear(Color.Transparent);
            SpriteBatch batch = new SpriteBatch(Core.graphics);

            batch.Begin(samplerState: SamplerState.PointClamp);

            batch.Draw(background, Vector2.Zero, Color.White);

            // Draw the hotbars
            int slotDist = 51;
            float scale;

            Vector2 itemPos = new Vector2(20, 308); // This is the starting position for drawing the primary hotbar's items.
            foreach (Item item in inventory.hotbarPrimary.items.Values)
            {
                scale = 38.0f / item.graphic.texture.Width; // 38.0f refers to the dimensions of the box to fit the texture into.

                item.graphic.draw(batch, itemPos, 0f, Vector2.Zero, scale, false);
                itemPos.X += slotDist;
            }

            itemPos = new Vector2(20, 388); // This is the starting position for drawing the secondary hotbar's items.
            foreach (Item item in inventory.hotbarSecondary.items.Values)
            {
                scale = 38.0f / item.graphic.texture.Width; // 38.0f refers to the dimensions of the box to fit the texture into.

                item.graphic.draw(batch, itemPos, 0f, Vector2.Zero, scale, false);
                itemPos.X += slotDist;
            }

            // Set up and draw the item page(s)
            int pageCount = 2;
            int itemNumber = 0;
            int maxPageItems = 40;
            GuiPlayerInventoryPage page;
            for (int i = 1; i <= pageCount; i++)
            {
                page = new GuiPlayerInventoryPage();
                for (int z = 0; z < maxPageItems; z++)
                {
                    page.addItem(inventory.getItem(itemNumber));
                    itemNumber++;
                }
                pages.Add(page);
            }

            pages[currentPage].Draw(batch);


            batch.End();
            Core.graphics.Reset();*/

        }

        public ItemSlot getSlotAt(Point pos, out int slotIndex)
        {
            //List<ItemSlot> slots = new List<ItemSlot>(pages[currentPage].slots);
            Rectangle slotBox;
            int count = 0;
            foreach (ItemSlot slot in itemSlots)
            {
                slotBox = new Rectangle(slot.box.X - 6, slot.box.Y - 6, slot.box.Width + 12, slot.box.Height + 12);
                if (slotBox.Contains(pos))
                {
                    slotIndex = count;
                    return slot;
                }
                count++;
            }
            slotIndex = -1;
            return null;
        }
        public Item getItemAt(Point pos, out int slotIndex)
        {
            //List<ItemSlot> slots = new List<ItemSlot>(pages[currentPage].slots);
            Rectangle slotBox;
            int count = 0;
            foreach (ItemSlot slot in itemSlots)
            {
                slotBox = new Rectangle(slot.box.X - 6, slot.box.Y - 6, slot.box.Width + 12, slot.box.Height + 12);
                if (slotBox.Contains(pos))
                {
                    slotIndex = count;
                    return slot.item;
                }
                count++;
            }
            slotIndex = -1;
            return null;
        }

        public void removeItem(int index)
        {
            if (index < 4)
            {
                // Handling for armor click
                return;
            }
            if (index < 9)
            {
                // Handling for primary hotbar click
                Core.player.inventory.hotbarPrimary.setItem(index - 4, null);
                return;
            }
            if (index < 14)
            {
                // Handling for secondary hotbar click
                Core.player.inventory.hotbarSecondary.setItem(index - 9, null);
                return;
            }

            Core.player.inventory.setItem(index - 14, null);
        }

        public void addItem(int index, Item item)
        {
            Item oldItem;
            if (index < 4)
            {
                // Handling for armor click
                return;
            }
            if (index < 9)
            {
                // Handling for primary hotbar click
                Core.player.inventory.hotbarPrimary.setItem(index - 4, item, out oldItem);
                cursorItem = oldItem;
                return;
            }
            if (index < 14)
            {
                // Handling for secondary hotbar click
                Core.player.inventory.hotbarSecondary.setItem(index - 9, item, out oldItem);
                cursorItem = oldItem;
                return;
            }

            Core.player.inventory.setItem(index - 14, item, out oldItem);
            cursorItem = oldItem;
        }

        public void nextPage()
        {
            currentPage++;
        }
        public void previousPage()
        {
            currentPage--;
        }


        public override void Draw(SpriteBatch batch)
        {
            //batch.Draw(texture, position, Color.White);

            //batch.Begin(samplerState: SamplerState.PointClamp);

            batch.Draw(background, position, Color.White);

            // Draw the hotbars
            foreach (ItemSlot slot in itemSlots)
            {
                slot.Draw(batch);
            }
            /*int slotDist = 51;
            float scale;

            Vector2 itemPos = new Vector2(position.X + 20, position.Y + 308); // This is the starting position for drawing the primary hotbar's items.
            foreach (Item item in inventory.hotbarPrimary.items.Values)
            {
                scale = 38.0f / item.graphic.texture.Width; // 38.0f refers to the dimensions of the box to fit the texture into.

                item.graphic.draw(batch, itemPos, 0f, Vector2.Zero, scale, false);
                itemPos.X += slotDist;
            }

            itemPos = new Vector2(position.X + 20, position.Y + 388); // This is the starting position for drawing the secondary hotbar's items.
            foreach (Item item in inventory.hotbarSecondary.items.Values)
            {
                scale = 38.0f / item.graphic.texture.Width; // 38.0f refers to the dimensions of the box to fit the texture into.

                item.graphic.draw(batch, itemPos, 0f, Vector2.Zero, scale, false);
                itemPos.X += slotDist;
            }*/

            //pages[currentPage].Draw(batch);


            if (cursorItem != null)
            {
                Graphic graphic = cursorItem.graphic;
                MouseState mState = Mouse.GetState();
                Vector2 mousePos = new Vector2(mState.X - (graphic.texture.Width / 2), mState.Y - (graphic.texture.Height / 2));
                float scale = 32.0f / graphic.texture.Width;
                graphic.draw(batch, mousePos, 0f, Vector2.Zero, scale);
            }

            //batch.End();
        }

    }
}
