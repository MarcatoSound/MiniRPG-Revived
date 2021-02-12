using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace BasicRPGTest_Mono.Engine.Utility
{
    public class YamlSection
    {
        private YamlMappingNode yaml;
        private string root;

        // TODO: Create setting and adding functionality.
        /// <summary>
        /// A wrapper object to simplify YAML data retrieval and traversal.
        /// </summary>
        /// <param name="root">The label of the top-level node</param>
        /// <param name="yaml">The YamlMappingNode that will be traversed</param>
        public YamlSection(string root, YamlMappingNode yaml)
        {
            this.root = root;
            if (root != "")
                this.yaml = (YamlMappingNode)yaml[root];
            else
                this.yaml = yaml;

        }
        public YamlSection(YamlMappingNode yaml)
        {
            this.root = "";
            this.yaml = yaml;
        }
        public YamlSection(string root)
        {
            this.root = root;
            this.yaml = new YamlMappingNode();
        }

        /// <summary>
        /// Gets the root/top-level node name of this YAML section.
        /// </summary>
        /// <returns>The top-level node name of this YAML section.</returns>
        public string getName()
        {
            return root;
        }
        /// <summary>
        /// Gets a value from the YAML section.
        /// </summary>
        /// <param name="path">The path to the value we are searching for, formatted as "key.subKey.subSubKey"</param>
        /// <param name="defaultValue">The default value to use if we can't find the path</param>
        /// <returns>The value at the end of the path; null if the value wasn't found</returns>
        public YamlNode get(string path)
        {
            YamlNode value = getDeepestValue(yaml, path);
            if (value != null)
                return value;
            return null;
        }
        /// <summary>
        /// Gets a string from the YAML section.
        /// </summary>
        /// <param name="path">The path to the value we are searching for, formatted as "key.subKey.subSubKey"</param>
        /// <param name="defaultValue">The default value to use if we can't find the path</param>
        /// <returns>The value at the end of the path; defaultValue is it wasn't found</returns>
        public string getString(string path, string defaultValue = "")
        {
            YamlNode value = getDeepestValue(yaml, path);
            if (value != null)
                return (string)value;
            return defaultValue;
        }
        /// <summary>
        /// Gets an integer from the YAML section.
        /// </summary>
        /// <param name="path">The path to the value we are searching for, formatted as "key.subKey.subSubKey"</param>
        /// <param name="defaultValue">The default value to use if we can't find the path</param>
        /// <returns>The value at the end of the path; defaultValue is it wasn't found</returns>
        public int getInt(string path, int defaultValue = 0)
        {
            YamlNode value = getDeepestValue(yaml, path);
            if (value != null)
                return int.Parse((string)value);
            return defaultValue;
        }
        /// <summary>
        /// Gets a double from the YAML section.
        /// </summary>
        /// <param name="path">The path to the value we are searching for, formatted as "key.subKey.subSubKey"</param>
        /// <param name="defaultValue">The default value to use if we can't find the path</param>
        /// <returns>The value at the end of the path; defaultValue is it wasn't found</returns>
        public double getDouble(string path, double defaultValue = 0)
        {
            YamlNode value = getDeepestValue(yaml, path);
            if (value != null)
                return double.Parse((string)value);
            return defaultValue;
        }
        public bool getBool(string path, bool defaultValue = false)
        {
            YamlNode value = getDeepestValue(yaml, path);
            if (value != null)
                return bool.Parse((string)value);
            return defaultValue;
        }

        /// <summary>
        /// Gets the deepest value of a YAML key string
        /// </summary>
        /// <param name="node">The YAML Mapping node (containing other YAML nodes) we searching for the path in</param>
        /// <param name="path">The path we are looking for the value at, formatted as "key.subKey.subSubKey"</param>
        /// <returns>The final value at the end of the path; null if any key was missing</returns>
        private YamlNode getDeepestValue(YamlMappingNode node, string path)
        {
            // Split the path string once into a key and a "sub-keys" string
            string[] paths = path.Split(".", 2);

            // Check if the current node has the first key in the provided path.
            if (node.Children.ContainsKey(paths[0]))
            {
                // We've found the end of the path; return the value.
                if (paths.Length == 1) return node[paths[0]];

                // We haven't found the end of the path, continue digging...
                YamlNode subNode = node[paths[0]];
                return getDeepestValue((YamlMappingNode)subNode, paths[1]);
            }

            // We couldn't find the key, so this is a dead path.
            return null;
        }



        // SETTERS AND ADDERS
        public void set(string path, YamlNode node)
        {

            buildPath(yaml, path, node);
        }
        public void setString(string path, string value)
        {

            buildPath(yaml, path, value);
        }
        public void setInt(string path, int value)
        {

            buildPath(yaml, path, value.ToString());
        }
        public void setDouble(string path, double value)
        {

            buildPath(yaml, path, value.ToString());
        }
        public void setBool(string path, bool value)
        {

            buildPath(yaml, path, value.ToString());
        }

        private void buildPath(YamlMappingNode node, string path, YamlNode value)
        {
            // Split the path string once into a key and a "sub-keys" string
            string[] paths = path.Split(".", 2);

            // We've found the end of the path; 
            if (paths.Length == 1)
            {
                node.Add(paths[0], value);
                return;
            }

            YamlNode subNode;
            // Check if the current node has the first key in the provided path.
            if (node.Children.ContainsKey(paths[0]))
            {
                subNode = node[paths[0]];

                buildPath((YamlMappingNode)subNode, paths[1], value);
                return;
            }

            // We haven't found the end of the path; create key and keep digging
            node.Add(paths[0], new YamlMappingNode());
            subNode = node[paths[0]];

            buildPath((YamlMappingNode)subNode, paths[1], value);

        }


        public override string ToString()
        {
            var serializer = new SerializerBuilder().Build();
            
            return serializer.Serialize(yaml);
        }

        public static implicit operator YamlMappingNode(YamlSection ys) => ys.yaml;
    }
}
