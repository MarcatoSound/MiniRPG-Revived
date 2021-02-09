using System;
using System.Collections.Generic;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Maps.Generation
{
    public static class GeneratorManager
    {
        private static List<Generator> generators;
        private static Dictionary<string, Generator> generatorsByName;

        static GeneratorManager()
        {
            generators = new List<Generator>();
            generatorsByName = new Dictionary<string, Generator>();
        }

        public static void add(Generator gen)
        {
            generators.Add(gen);
            generatorsByName.Add(gen.name, gen);
        }

        public static Generator get(int i)
        {
            if (i > generators.Count - 1) return null;
            return generators[i];
        }
        public static Generator getByNamespace(string name)
        {
            if (name != null && generatorsByName.ContainsKey(name))
                return generatorsByName[name];

            return null;
        }

        public static List<Generator> getItems()
        {
            return generators;
        }

        public static void Clear()
        {
            generators.Clear();
        }
    }
}
