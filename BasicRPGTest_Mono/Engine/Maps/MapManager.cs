using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class MapManager
    {
        public static List<Map> maps;
        public static Map activeMap { get; set; }
        static MapManager()
        {
            maps = new List<Map>();
        }

        public static void add(Map map)
        {
            if (maps.Count == 0) activeMap = map;
            maps.Add(map);
        }
        public static Map get(int i)
        {
            return maps[i];
        }

    }
}
