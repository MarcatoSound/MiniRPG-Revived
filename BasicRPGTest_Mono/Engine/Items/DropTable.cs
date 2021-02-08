using BasicRPGTest_Mono.Engine.Datapacks;
using BasicRPGTest_Mono.Engine.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine.Items
{
    public class DropTable
    {
        public string name;
        private List<ItemDrop> drops = new List<ItemDrop>();
        public int min = 1;
        public int max = 1;

        public DropTable() { }
        public DropTable(List<ItemDrop> drops)
        {
            this.drops = drops;
        }
        public DropTable(DataPack pack, YamlSection config)
        {
            this.name = config.getName();
            this.min = config.getInt("min_drops", 1);
            this.max = config.getInt("max_drops", 1);

            YamlNode dropsYaml = config.get("drops");
            if (dropsYaml != null && dropsYaml.NodeType == YamlNodeType.Sequence)
            {
                YamlSequenceNode sequence = (YamlSequenceNode)dropsYaml;

                foreach (var entry in sequence)
                {
                    if (entry.NodeType != YamlNodeType.Mapping) continue;
                    YamlMappingNode map = (YamlMappingNode)entry;

                    YamlSection dropConfig = new YamlSection(map);

                    string itemName = dropConfig.getString("item");
                    System.Diagnostics.Debug.WriteLine($"// ││├┬ Processing drop entry for item '{itemName}'...");
                    ParentItem item = ItemManager.getByNamespace(itemName);
                    if (item == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"// │││└╾ ERR: Unable to find item '{itemName}' when creating drop table! Skipping...");
                        continue;
                    }
                    drops.Add(new ItemDrop(item, config.getInt("min", 1), config.getInt("max", 1), config.getDouble("weight", 1)));
                    System.Diagnostics.Debug.WriteLine($"// │││└╾ SUCCESS!");
                }
            }
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
