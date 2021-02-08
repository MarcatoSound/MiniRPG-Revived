using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Items
{
    public static class DropTableManager
    {
        private static List<DropTable> tables;
        private static Dictionary<string, DropTable> tablesByName;

        static DropTableManager()
        {
            tables = new List<DropTable>();
            tablesByName = new Dictionary<string, DropTable>();
        }

        public static void add(DropTable table)
        {
            tables.Add(table);
            tablesByName.Add(table.name, table);
        }

        public static DropTable get(int i)
        {
            if (i > tables.Count - 1) return null;
            return tables[i];
        }
        public static DropTable getByNamespace(string name)
        {
            if (name != null && tablesByName.ContainsKey(name))
                return tablesByName[name];

            return new DropTable();
        }

        public static List<DropTable> getItems()
        {
            return tables;
        }

        public static void Clear()
        {
            tables.Clear();
        }
    }
}
