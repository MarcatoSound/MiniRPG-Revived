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

        public Item getItem(int slot)
        {
            if (slot > items.Count - 1) return null;
            return items[slot];
        }

        public void setItem(int slot, Item item)
        {
            if (getItem(slot) != null) items.TryRemove(slot, out _);
            items.TryAdd(slot, item);
        }
        public void setItem(int slot, Item item, out Item oldItem)
        {
            if (getItem(slot) != null) items.TryRemove(slot, out oldItem);
            else oldItem = null;
            items.TryAdd(slot, item);
        }

    }
}
