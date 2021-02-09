using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Datapacks
{
    public static class DataPackManager
    {
        private static List<DataPack> packs;
        private static Dictionary<string, DataPack> packsByName;

        static DataPackManager()
        {
            packs = new List<DataPack>();
            packsByName = new Dictionary<string, DataPack>();
        }

        public static void add(DataPack pack)
        {
            packs.Add(pack);
            packsByName.Add(pack.name, pack);
        }

        public static DataPack get(int i)
        {
            if (i > packs.Count - 1) return null;
            return packs[i];
        }
        public static DataPack getByNamespace(string name)
        {
            if (name != null && packsByName.ContainsKey(name))
                return packsByName[name];

            return null;
        }

        public static List<DataPack> getItems()
        {
            return packs;
        }

        public static void loadItems()
        {
            foreach (DataPack pack in packs)
            {
                pack.loadItems();
            }
        }
        public static void loadTiles()
        {
            foreach (DataPack pack in packs)
            {
                pack.loadTiles();
            }
        }
        public static void loadBiomes()
        {
            // TODO: Make this do the thing.
        }
        public static void loadDropTables()
        {
            foreach (DataPack pack in packs)
            {
                pack.loadDropTables();
            }
        }
        public static void loadEntities()
        {
            foreach (DataPack pack in packs)
            {
                pack.loadEntities();
            }
        }

        public static void Clear()
        {
            packs.Clear();
        }
    }
}
