using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine.Inventories
{
    public class Hotbar : Inventory
    {
        public Item hand
        {
            get
            {
                return getItem(selectedSlot);
            }
        }

        private int _selectedSlot;
        public int selectedSlot
        {
            get { return _selectedSlot; }
            set
            {
                if (value > maxItems - 1 || value < 0) return;
                _selectedSlot = value;
            }
        }

        public Hotbar()
        {
            maxItems = 5;
            for (int i = 0; i < maxItems; i++)
            {
                setItem(i, null);
            }
        }
        public Hotbar(YamlSection data)
        {
            maxItems = data.getInt("size", 5);
            for (int i = 0; i < maxItems; i++)
            {
                YamlSection itemData = data.getSection($"slots.{i}");
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

        public void next()
        {
            selectedSlot++;
        }
        public void previous()
        {
            selectedSlot--;
        }

        public void setSlot(int i)
        {
            selectedSlot = i;
        }
    }
}
