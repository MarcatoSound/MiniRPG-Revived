using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRPGTest_Mono.Engine.Entities.Tags
{
    public interface ITaggable
    {
        public TagContainer Tags { get; }
    }

    public static class ITaggableExtensions
    {
        public static void AddTag<T>(this ITaggable taggable, string key, T value)
        {
            taggable.Tags.Add(key, value);
        }

        public static void RemoveTag(this ITaggable taggable, string key)
        {
            taggable.Tags.Remove(key);
        }

        public static T GetTag<T>(this ITaggable taggable, string key)
        {
            return taggable.Tags.Get<T>(key);
        }

        public static bool HasTag(this ITaggable taggable, string key)
        {
            return taggable.Tags.Has(key);
        }

        public static List<T> GetTagsOf<T>(this ITaggable taggable)
        {
            return taggable.Tags.GetAllOf<T>();
        }

        public static dynamic GetTags(this ITaggable taggable)
        {
            return taggable.Tags.GetAll();
        }
    }
}
