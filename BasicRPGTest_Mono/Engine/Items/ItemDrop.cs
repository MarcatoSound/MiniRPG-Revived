using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class ItemDrop
    {
        public ParentItem item { get; private set; }

        public int min;
        public int max;

        public double weight;

        public ItemDrop(ParentItem item, int min = 1, int max = 1, double weight = 1)
        {
            this.item = item;
            this.min = min;
            this.max = max;
            this.weight = weight;
        }

        public bool tryDrop(Map map, Vector2 pos)
        {
            if (!Utility.Util.randomBool(weight)) return false;

            Random rand = new Random();
            int quantity = rand.Next(min, max);
            map.spawnItem(new Item(item, quantity), pos);
            return true;
        }
        public void drop(Map map, Vector2 pos)
        {
            Random rand = new Random();
            int quantity = rand.Next(min, max);
            map.spawnItem(new Item(item, quantity), pos);
        }
    }
}
