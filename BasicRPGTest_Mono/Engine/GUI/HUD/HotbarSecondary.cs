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

            slotTexture = Utility.Util.getSpriteFromSet(HudManager.tileset, 0, 4, 24);
            selectedTexture = Utility.Util.getSpriteFromSet(HudManager.tileset, 1, 4, 24);

            slotDist = slotTexture.Width + 2;
            primary = (HotbarPrimary)HudManager.getByName("hotbarprimary");
        }

        public override void Update()
        {
            if (screenPos.X != primary.screenPos.X)
                screenPos.X = primary.screenPos.X;
            if (screenPos.Y != primary.screenPos.Y - 35)
                screenPos.Y = primary.screenPos.Y - 35;
            //if (screenPos.X != Core.game.Window.ClientBounds.Width / 16)
            //    screenPos.X = Core.game.Window.ClientBounds.Width / 16;
            //if (screenPos.Y != Core.game.Window.ClientBounds.Height - (Core.game.Window.ClientBounds.Height / 7))
            //    screenPos.Y = Core.game.Window.ClientBounds.Height - (Core.game.Window.ClientBounds.Height / 7);

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
            Vector2 itemOffset = new Vector2(-2, -2);
            //Vector2 itemPos = new Vector2(pos.X - (slotTexture.Width / 2) + pos.Y - (slotTexture.Height / 2));
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
                    scale = 20.0f / item.graphic.texture.Width;
                    item.graphic.draw(batch, pos, 0f, itemOffset, scale, false);
                }

                pos.X += slotDist;
            }
        }
    }
}
