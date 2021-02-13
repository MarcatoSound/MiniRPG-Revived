using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Inventories
{
    public class PlayerInventory : Inventory
    {
        public Item mainhand
        {
            get { return hotbarPrimary.getItem(hotbarPrimary.selectedSlot); }
        }
        public Item offhand
        {
            get { return hotbarSecondary.getItem(hotbarSecondary.selectedSlot); }
        }

        public Hotbar hotbarPrimary;
        public Hotbar hotbarSecondary;

        public PlayerInventory()
        {
            hotbarPrimary = new Hotbar();
            hotbarSecondary = new Hotbar();

            maxItems = 80;
            for (int i = 0; i < maxItems; i++)
            {
                setItem(i, null);
            }
        }
        public PlayerInventory(YamlSection data)
        {
            hotbarPrimary = new Hotbar(data.getSection("primary_hotbar"));
            hotbarSecondary = new Hotbar(data.getSection("secondary_hotbar"));

            maxItems = data.getInt("inventory.size", 5);
            for (int i = 0; i < maxItems; i++)
            {
                YamlSection itemData = data.getSection($"inventory.slots.{i}");
                if (itemData == null)
                    setItem(i, null);
                else
                {
                    Item item = new Item(itemData);
                    if (item.parent == null)
                    {
                        setItem(i, null);
                        continue;
                    }
                    setItem(i, item);
                }
            }

        }

        public static implicit operator YamlSection(PlayerInventory inv)
        {
            YamlSection config = new YamlSection($"player_inventory");

            config.set("primary_hotbar", (YamlSection)inv.hotbarPrimary);
            config.set("secondary_hotbar", (YamlSection)inv.hotbarSecondary);
            config.set("inventory", (YamlSection)(Inventory)inv);

            return config;
        }

    }
}
