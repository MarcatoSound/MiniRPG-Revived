using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRPGTest_Mono.Engine.Entities.Tags
{
    /// <summary>
    /// A wrapped dictionary that contains dynamic content that can be attached to another object.
    /// </summary>
    public class TagContainer
    {
        private Dictionary<string, dynamic> tags;

        public TagContainer() { }

        // Tragically, C# does not support generic typed indexers. Maybe someday... :copium:
        /*public EntityTag<T> this<T>[string name]
        {
            get { return (EntityTag<T>)tags[name]; }
        }*/

        public void Add<T>(string key, T value) // TODO: Perform type constraints here!
        {
            tags.Add(key, value);
        }

        public void Remove(string key)
        {
            tags.Remove(key);
        }

        public bool Has(string key) { return tags.ContainsKey(key); }

        public T? Get<T>(string key)
        {
            if (!tags.ContainsKey(key)) return default;
            return (T)tags[key];
        }

        public List<T> GetAllOf<T>()
        {
            List<T> tags = new();

            foreach (var itag in this.tags.Values)
            {
                if (itag is not T tag) continue;
                tags.Add(tag);
            }

            return tags;
        }

        public List<dynamic> GetAll()
        {
            return tags.Values.ToList();
        }
    }
}
