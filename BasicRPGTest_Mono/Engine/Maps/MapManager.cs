using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine
{
    public static class MapManager
    {
        private static List<Map> maps;
        private static Dictionary<string, Map> mapsByName;
        public static Map activeMap { get; set; }
        static MapManager()
        {
            maps = new List<Map>();
            mapsByName = new Dictionary<string, Map>();
        }

        /// <summary>
        /// Adds and stores a map in the list of loaded maps.
        /// </summary>
        /// <param name="tile">The map object that is being stored.</param>
        public static void add(Map map)
        {
            // If this is the first map being loaded, make it the active map.
            /* TODO: Rewrite this later when we have multiple maps to handle; 
             * probably use a parameter in the "world.json" save file to remember the active map.
             */
            if (maps.Count == 0) activeMap = map;
            maps.Add(map);
            mapsByName.Add(map.name, map);
        }
        /// <summary>
        /// Retrieves a map object from the list of loaded maps.
        /// </summary>
        /// <param name="index">The numerical index we are retrieving the map from.</param>
        /// <returns>The map matching the requested index.</returns>
        public static Map get(int index)
        {
            return maps[index];
        }

        /// <summary>
        /// Retrieves a map object from the list of loaded maps using its namespace.
        /// </summary>
        /// <param name="name">The registered namespace of the map.</param>
        /// <returns>The map matching the "name" parameter provided. NULL if it wasn't found.</returns>
        public static Map getByName(string name)
        {
            if (name != null && mapsByName.ContainsKey(name))
                return mapsByName[name];

            return null;
        }

        public static void clear()
        {
            maps.Clear();
            activeMap.Clear();
            activeMap = null;
        }

    }
}
