using BasicRPGTest_Mono.Engine.Inventories;
using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.HUD
{
    public class HotbarSecondary : HudElement
    {

        public HotbarPrimary primary;

        public Hotbar hotbar;

        private Texture2D slotTexture;
        private Texture2D selectedTexture;

        private int slotDist;

        public HotbarSecondary(Vector2 screenPos) :
            base("hotbarsecondary", screenPos)
        {
            System.Diagnostics.Debug.WriteLine("Hotbar 2 position: " + screenPos);

            slotTexture = Utility.Util.getSpriteFromSet(HudManager.tileset, 0, 4, 32);
            selectedTexture = Utility.Util.getSpriteFromSet(HudManager.tileset, 1, 4, 32);

            slotDist = slotTexture.Width + 2;

            primary = (HotbarPrimary)HudManager.getByName("hotbarprimary");
        }

        public override void Update()
        {
            if (screenPos.X != primary.screenPos.X)
                screenPos.X = primary.screenPos.X;
            if (screenPos.Y != primary.screenPos.Y - 40)
                screenPos.Y = primary.screenPos.Y - 40;

            if (Core.player == null) return;
            if (Core.player.inventory == null) return;

            if (hotbar != Core.player.inventory.hotbarSecondary)
            {
                hotbar = Core.player.inventory.hotbarSecondary;
                primary = (HotbarPrimary)HudManager.getByName("hotbarprimary");
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            if (hotbar == null) return;

            Texture2D texture;
            Vector2 pos = new Vector2(screenPos.X, screenPos.Y);
            Vector2 itemPos;
            Item item;
            float scale;
            for (int i = 0; i < hotbar.maxItems; i++)
            {
                item = hotbar.getItem(i);
                if (i == hotbar.selectedSlot)
                    texture = selectedTexture;
                else
                    texture = slotTexture;

                batch.Draw(texture, pos, Color.White);

                if (item != null)
                {
                    scale = 28.0f / item.graphic.texture.Width;
                    itemPos.X = pos.X + 2;
                    itemPos.Y = pos.Y + 2;
                    item.graphic.draw(batch, itemPos, 0f, Vector2.Zero, scale);
                }

                pos.X += slotDist;
            }
        }
    }
}
