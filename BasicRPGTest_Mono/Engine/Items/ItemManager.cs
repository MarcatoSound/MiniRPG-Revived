using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public static class ItemManager
    {
        public static List<ParentItem> items;

        static ItemManager()
        {
            items = new List<ParentItem>();
        }

        public static void add(ParentItem item)
        {
            items.Add(item);
        }

        public static ParentItem get(int i)
        {
            if (i > items.Count - 1) return null;
            return items[i];
        }
        public static ParentItem getByNamespace(string name)
        {
            foreach (ParentItem item in items)
            {
                if (item.name == name) return item;
            }

            return null;
        }

        public static List<ParentItem> getTiles()
        {
            return items;
        }

        public static void Clear()
        {
            items.Clear();
        }
    }
}
