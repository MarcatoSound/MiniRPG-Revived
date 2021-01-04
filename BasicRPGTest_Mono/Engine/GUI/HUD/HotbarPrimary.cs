using BasicRPGTest_Mono.Engine.Inventories;
using BasicRPGTest_Mono.Engine.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.GUI.HUD
{
    public class HotbarPrimary : HudElement
    {
        public Hotbar hotbar;

        private Texture2D slotTexture;
        private Texture2D selectedTexture;

        private int slotDist;

        public HotbarPrimary() : 
            base("hotbarprimary", new Vector2(Core.game.Window.ClientBounds.Width / 16, Core.game.Window.ClientBounds.Height - (Core.game.Window.ClientBounds.Height / 10)))
        {
            System.Diagnostics.Debug.WriteLine("Hotbar position: " + screenPos);

            slotTexture = Utility.Util.getSpriteFromSet(HudManager.tileset, 0, 0, 48);
            selectedTexture = Utility.Util.getSpriteFromSet(HudManager.tileset, 0, 1, 48);

            slotDist = slotTexture.Width + 2;
        }

        public override void Update()
        {
            if (screenPos.X != Core.game.Window.ClientBounds.Width / 16)
                screenPos.X = Core.game.Window.ClientBounds.Width / 16;
            if (screenPos.Y != Core.game.Window.ClientBounds.Height - (Core.game.Window.ClientBounds.Height / 10))
                screenPos.Y = Core.game.Window.ClientBounds.Height - (Core.game.Window.ClientBounds.Height / 10);

            if (Core.player == null) return;
            if (Core.player.inventory == null) return;

            if (hotbar != Core.player.inventory.hotbarPrimary)
                hotbar = Core.player.inventory.hotbarPrimary;
        }

        public override void Draw(SpriteBatch batch)
        {
            if (hotbar == null) return;

            Texture2D texture;
            Vector2 pos = new Vector2(screenPos.X, screenPos.Y);
            Vector2 itemOffset = new Vector2(-5, -5);
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
                    scale = 38.0f / item.graphic.texture.Width;
                    item.graphic.draw(batch, pos, 0f, itemOffset, scale, false);
                }

                pos.X += slotDist;
            }
        }

    }


}
