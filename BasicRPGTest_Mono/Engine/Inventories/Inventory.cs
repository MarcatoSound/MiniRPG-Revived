using BasicRPGTest_Mono.Engine.Items;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public class Inventory
    {
        private ConcurrentDictionary<int, Item> items { get; set; }
        public int maxItems;

        public Inventory()
        {
            items = new ConcurrentDictionary<int, Item>();
        }

        /// <summary>
        /// Gets all the items in this inventory as a list.
        /// </summary>
        /// <returns>A list of items from the inventory.</returns>
        public List<Item> getAllItems()
        {
            return new List<Item>(items.Values);
        }

        /// <summary>
        /// Gets and item at a specific slot index.
        /// Slots are ordered left-to-right, then top-to-bottom.
        /// </summary>
        /// <param name="slot">The slot index we are retrieving from.</param>
        /// <returns>The item located in the specified inventory slot; null if the slot is empty.</returns>
        public Item getItem(int slot)
        {
            if (items.ContainsKey(slot))
                return items[slot];

            return null;
        }

        /// <summary>
        /// Attempts to add an item to the inventory, merging the quantity with existing
        /// instances of the same item, and adding to the first available slot otherwise.
        /// </summary>
        /// <param name="item">The new item we are adding to the inventory.</param>
        public void addItem(Item item)
        {
            if (item == null) return;

            // Loop through existing items to see if there's already one in the inventory.
            foreach (Item i in items.Values)
            {
                if (i == null) continue;

                // If there is a matching item here, try merging the stacks.
                if (i.id == item.id)
                {
                    int newQuantity = i.quantity + item.quantity;

                    // If the combined stack size is larger than the maximum, max out that item's
                    // quantity and search for another matching item in the inventory.
                    if (newQuantity > i.maxStackSize)
                    {
                        item.quantity = newQuantity - i.maxStackSize;
                        i.quantity = i.maxStackSize;
                        continue;
                    }
                    i.quantity = newQuantity;
                    return;
                }
            }

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

        /// <summary>
        /// Place an item at a specific slot index, hard-replacing the old item.
        /// </summary>
        /// <param name="slot">The slot index. (Left-to-right, top-to-bottom)</param>
        /// <param name="item">The new item we are putting in this slot.</param>
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
        /// <summary>
        /// Place an item at a specific slot index, while also providing the item we replaced.
        /// (Null if there was no item replaced.)
        /// </summary>
        /// <param name="slot">The slot index. (Left-to-right, top-to-bottom)</param>
        /// <param name="newItem">The new item we are putting in this slot.</param>
        /// <param name="oldItem">The old item that used to occupy this slot.</param>
        public void setItem(int slot, Item newItem, out Item oldItem)
        {

            items.TryRemove(slot, out oldItem);

            // If the item in this slot has the same id as the provided item...
            if (oldItem != null && oldItem.id == newItem.id)
            {
                // Updates the quantity of the item being placed to the max stack size, 
                // while the player picks up the remainder.
                int newQuantity = oldItem.quantity + newItem.quantity;
                if (newQuantity > oldItem.maxStackSize)
                {
                    // If the new quantity would be greater than the max possible stack quantity...
                    oldItem.quantity = newQuantity - oldItem.maxStackSize;
                    newItem.quantity = oldItem.maxStackSize;
                } else
                {
                    // Otherwise, everything is fine. Merge the two items.
                    newItem.quantity = newQuantity;
                    oldItem = null;
                }
            }

            items.TryAdd(slot, newItem);
        }

        /// <summary>
        /// Retrieves the index of the first empty slot in this inventory.
        /// </summary>
        /// <returns>The first empty slot index. -1 if there are no empty slots.</returns>
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
