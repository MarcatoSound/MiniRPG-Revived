using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public static class ItemManager
    {
        public static List<Item> items;

        static ItemManager()
        {
            items = new List<Item>();
        }

        public static void add(Item item)
        {
            items.Add(item);
        }

        public static Item get(int i)
        {
            if (i > items.Count - 1) return null;
            return items[i];
        }
        public static Item getByNamespace(string name)
        {
            foreach (Item item in items)
            {
                if (item.name == name) return item;
            }

            return null;
        }

        public static List<Item> getTiles()
        {
            return items;
        }

        public static void Clear()
        {
            items.Clear();
        }
    }
}
