using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public static class ItemManager
    {
        private static List<ParentItem> items;
        private static Dictionary<string, ParentItem> itemsByName;

        static ItemManager()
        {
            items = new List<ParentItem>();
            itemsByName = new Dictionary<string, ParentItem>();
        }
        
        public static void add(ParentItem item)
        {
            items.Add(item);
            itemsByName.Add(item.name, item);
        }

        public static ParentItem get(int i)
        {
            if (i > items.Count - 1) return null;
            return items[i];
        }
        public static ParentItem getByNamespace(string name)
        {
            if (name != null && itemsByName.ContainsKey(name))
                return itemsByName[name];

            return null;
        }

        public static List<ParentItem> getItems()
        {
            return items;
        }

        public static void Clear()
        {
            items.Clear();
        }
    }
}
