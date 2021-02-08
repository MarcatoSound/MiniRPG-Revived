using BasicRPGTest_Mono.Engine.Items;
using BasicRPGTest_Mono.Engine.Utility;
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
            foreach (string file in files)
            {
                System.Diagnostics.Debug.WriteLine($"Files: {file}");
            }
            if (!files.Contains($"{path}\\pack.yml"))
            {
                System.Diagnostics.Debug.WriteLine($"Error!! Invalid pack: {path}");
                return;
            }

            packPath = path;

            loadInfo();
        }

        private void loadInfo()
        {
            System.Diagnostics.Debug.WriteLine($"// LOADING PACK '{packPath}'... //");

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
            System.Diagnostics.Debug.WriteLine($"// LOADING ITEMS FOR PACK '{name}'... //");

            string path = $"{packPath}\\items";
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                System.Diagnostics.Debug.WriteLine($"// -- Reading file {file}");
                var reader = new StreamReader(file);
                var input = new StringReader(reader.ReadToEnd());

                YamlStream yaml = new YamlStream();
                yaml.Load(input);

                YamlMappingNode mapping = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var child in mapping.Children)
                {
                    YamlMappingNode itemYaml = new YamlMappingNode(child);
                    System.Diagnostics.Debug.WriteLine($"// --- Loading item {child.Key}");
                    YamlSection config = new YamlSection((string)child.Key, itemYaml);

                    ItemManager.add(new ParentItem(this, config));
                }
            }
        }
    }
}
