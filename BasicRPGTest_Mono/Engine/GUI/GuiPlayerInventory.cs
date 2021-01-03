using BasicRPGTest_Mono.Engine.Inventories;
using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI
{

    public class GuiPlayerInventory : GuiWindow
    {
        public Texture2D background;
        public RenderTarget2D texture;
        public Vector2 position;
        public PlayerInventory inventory;
        public List<GuiPlayerInventoryPage> pages;

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
        }

        public void updateGui()
        {
            inventory = Core.player.inventory;

            // Generate the GUI window
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
                System.Diagnostics.Debug.WriteLine($"Scale for {item.displayName}: {scale}" );

                item.graphic.draw(batch, itemPos, 0f, Vector2.Zero, scale, false);
                itemPos.X += slotDist;
            }

            itemPos = new Vector2(20, 388); // This is the starting position for drawing the secondary hotbar's items.
            foreach (Item item in inventory.hotbarSecondary.items.Values)
            {
                scale = 38.0f / item.graphic.texture.Width; // 38.0f refers to the dimensions of the box to fit the texture into.
                System.Diagnostics.Debug.WriteLine($"Scale for {item.displayName}: {scale}");

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
                    //System.Diagnostics.Debug.WriteLine("Item number: " + itemNumber);
                    itemNumber++;
                }
                pages.Add(page);
            }

            pages[currentPage].Draw(batch);
            //System.Diagnostics.Debug.WriteLine("Page item count: " + pages[currentPage].items.Count);


            batch.End();
            Core.graphics.Reset();

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
            batch.Begin();

            batch.Draw(texture, position, Color.White);

            batch.End();
        }

    }
}
