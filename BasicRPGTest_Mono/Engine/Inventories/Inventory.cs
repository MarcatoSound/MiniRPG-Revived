using BasicRPGTest_Mono.Engine.Items;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Inventory
    {
        public ConcurrentDictionary<int, Item> items;
        public int maxItems;

        public Inventory()
        {
            items = new ConcurrentDictionary<int, Item>();
        }

        public Item getItem(int slot)
        {
            Item item;
            try
            {
                item = items[slot];
            } catch (KeyNotFoundException)
            {
                return null;
            }
            return item;
        }

        public void addItem(Item item)
        {
            int slot = getFirstEmpty();
            //System.Diagnostics.Debug.WriteLine("First empty: " + slot);
            if (maxItems != 0 && slot + 1 > maxItems)
            {
                // Put an error message here
                System.Diagnostics.Debug.WriteLine("Canceled adding item " + item.displayName);
                return;
            }

            items.TryRemove(slot, out _);
            items.TryAdd(slot, item);
        }

        public void setItem(int slot, Item item)
        {
            if (maxItems != 0 && slot + 1 > maxItems)
            {
                // Put an error message here
                return;
            }

            items.TryRemove(slot, out _);
            items.TryAdd(slot, item);
        }
        public void setItem(int slot, Item item, out Item oldItem)
        {
            items.TryRemove(slot, out oldItem);
            items.TryAdd(slot, item);
        }

        // Returns -1 if there are no empty slots.
        public int getFirstEmpty()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (getItem(i) == null) return i;
            }

            return -1;
        }

    }
}
