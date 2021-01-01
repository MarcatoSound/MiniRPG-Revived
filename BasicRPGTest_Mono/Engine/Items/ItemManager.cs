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
    }
}
