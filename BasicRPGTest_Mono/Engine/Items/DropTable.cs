using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class DropTable
    {
        private List<ItemDrop> drops = new List<ItemDrop>();
        public int min = 1;
        public int max = 1;

        public DropTable() { }
        public DropTable(List<ItemDrop> drops)
        {
            this.drops = drops;
        }

        public void add(ItemDrop drop)
        {
            drops.Add(drop);
        }
        public List<ItemDrop> getDrops()
        {
            return drops;
        }

        public ItemDrop selectDrop()
        {
            // O(n) performance
            int totalRatio = 0;

            foreach (ItemDrop s in drops)
                totalRatio += Convert.ToInt32(s.weight * 100);

            Random random = new Random();
            int x = random.Next(0, totalRatio);

            int iteration = 0; // so you know what to do next
            foreach (ItemDrop s in drops)
            {
                if ((x -= Convert.ToInt32(s.weight * 100)) < 0)
                    break;
                iteration++;
            }

            return drops[iteration];
        }
        public void dropItems(Map map, Vector2 pos)
        {
            if (drops.Count < 1) return;
            Random rand = new Random();
            int quantity = rand.Next(min, max);

            ItemDrop drop;
            Vector2 dropPos;
            for (int i = 0; i < quantity; i++)
            {
                drop = selectDrop();
                if (drop == null) continue;
                dropPos = Utility.Util.randomizePosition(pos, 8);
                drop.drop(map, dropPos);
            }
        }

    }
}
