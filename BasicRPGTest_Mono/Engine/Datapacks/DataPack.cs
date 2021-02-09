using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Maps;
using BasicRPGTest_Mono.Engine.Utility;
using RPGEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace BasicRPGTest_Mono.Engine.Datapacks
{
    public class DataPack
    {
        public string packPath;

        public string name;
        public string description;
        public string author;
        public string version;
        public DataPack(string path)
        {
            List<string> files = new List<string>(Directory.GetFiles(path));
            if (!files.Contains($"{path}\\pack.yml"))
            {
                Console.WriteLine($"Error!! Invalid pack: {path}");
                return;
            }

            packPath = path;

            loadInfo();
        }

        private void loadInfo()
        {
            Console.WriteLine($"// LOADING PACK '{packPath}'... //");

            var reader = new StreamReader($"{packPath}\\pack.yml");
            var input = new StringReader(reader.ReadToEnd());

            YamlStream yaml = new YamlStream();
            yaml.Load(input);

            YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

            name = (string)mapping.Children[new YamlScalarNode("pack_name")];
            description = (string)mapping.Children[new YamlScalarNode("pack_description")];
            author = (string)mapping.Children[new YamlScalarNode("pack_author")];
            version = (string)mapping.Children[new YamlScalarNode("pack_version")];
        }

        public void loadItems()
        {
            Console.WriteLine($"// LOADING ITEMS FOR PACK '{name}'... //");

            string path = $"{packPath}\\items";
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Console.WriteLine($"// ├┬ Reading file {file}...");
                var reader = new StreamReader(file);
                var input = new StringReader(reader.ReadToEnd());

                YamlStream yaml = new YamlStream();
                yaml.Load(input);

                YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var child in mapping.Children)
                {
                    YamlMappingNode itemYaml = new YamlMappingNode(child);
                    Console.WriteLine($"// │├┬ Loading item {child.Key}");
                    YamlSection config = new YamlSection((string)child.Key, itemYaml);

                    ItemManager.add(new ParentItem(this, config));
                    Console.WriteLine($"// ││└╾ SUCCESS");
                }
                Console.WriteLine($"// │└╾ Finished reading file.");
            }

            Console.WriteLine($"// └╾ Finished loading items.");
        }
        public void loadTiles()
        {
            Console.WriteLine($"// LOADING TILES FOR PACK '{name}'... //");

            string path = $"{packPath}\\tiles";
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Console.WriteLine($"// ├┬ Reading file {file}...");
                var reader = new StreamReader(file);
                var input = new StringReader(reader.ReadToEnd());

                YamlStream yaml = new YamlStream();
                yaml.Load(input);

                YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var child in mapping.Children)
                {
                    YamlMappingNode itemYaml = new YamlMappingNode(child);
                    Console.WriteLine($"// │├┬ Loading tile {child.Key}");
                    YamlSection config = new YamlSection((string)child.Key, itemYaml);

                    TileManager.add(new Tile(this, config));
                    Console.WriteLine($"// ││└╾ SUCCESS");
                }
                Console.WriteLine($"// │└╾ Finished reading file.");
            }

            Console.WriteLine($"// └╾ Finished loading tiles.");
        }
        public void loadDropTables()
        {
            Console.WriteLine($"// LOADING DROPTABLES FOR PACK '{name}'... //");

            string path = $"{packPath}\\droptables";
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Console.WriteLine($"// ├┬ Reading file {file}");
                var reader = new StreamReader(file);
                var input = new StringReader(reader.ReadToEnd());

                YamlStream yaml = new YamlStream();
                yaml.Load(input);

                YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var child in mapping.Children)
                {
                    YamlMappingNode itemYaml = new YamlMappingNode(child);
                    Console.WriteLine($"// │├┬ Loading droptable {child.Key}...");
                    YamlSection config = new YamlSection((string)child.Key, itemYaml);

                    DropTableManager.add(new DropTable(this, config));
                    Console.WriteLine($"// ││└╾ Finished loading droptable.");
                }

                Console.WriteLine($"// │└╾ Finished reading file.");
            }

            Console.WriteLine($"// └╾ Finished loading droptables.");
        }
        public void loadEntities()
        {
            Console.WriteLine($"// LOADING ENTITIES FOR PACK '{name}'... //");

            string path = $"{packPath}\\entities";
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                Console.WriteLine($"// ├┬ Reading file {file}");
                var reader = new StreamReader(file);
                var input = new StringReader(reader.ReadToEnd());

                YamlStream yaml = new YamlStream();
                yaml.Load(input);

                YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var child in mapping.Children)
                {
                    YamlMappingNode itemYaml = new YamlMappingNode(child);
                    Console.WriteLine($"// │├┬ Loading entity {child.Key}...");
                    YamlSection config = new YamlSection((string)child.Key, itemYaml);

                    EntityManager.add(new LivingEntity(this, config));
                    Console.WriteLine($"// ││└╾ Finished loading entity.");
                }

                Console.WriteLine($"// │└╾ Finished reading file.");
            }

            Console.WriteLine($"// └╾ Finished loading entities.");
        }
    }
}
