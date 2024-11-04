using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicRPGTest_Mono.Engine.Entities.Tags
{
    /// <summary>
    /// Underlying tag type for wildcarding. Use one of the implementations below.
    /// </summary>
    /// <seealso cref="Tag{T}"/>
    /// <seealso cref="ListTag{T}"/>
    public interface ITag { }

    /// <summary>
    /// Stores a single primitive value.
    /// </summary>
    /// <typeparam name="T">A primitive or string.</typeparam>
    [ProtoContract]
    public class Tag<T> : ITag where T : IConvertible// TODO: Constrained to primitives... find a way to allow more complicated data types to be stored as tags!
    {
        [ProtoMember(1)]
        public virtual T Value { get; set; }

        public Tag(T obj)
        {
            Value = obj;
        }
        // Empty constructor to appease the fickle ProtoBuf gods, just in case.
        protected Tag() { }

        public static Tag<T> From(T obj)
        {
            return new Tag<T>(obj);
        }
    }

    /// <summary>
    /// Stores a collection of a primitive value.
    /// </summary>
    /// <typeparam name="T">A primitive or string.</typeparam>
    [ProtoContract]
    public class ListTag<T> : Tag<T> where T : IConvertible
    {
        [ProtoMember(1)]
        public new ICollection<T> Value { get; set; }

        public ListTag(ICollection<T> collection)
        {
            Value = collection;
        }
        protected ListTag() { }

        public static ListTag<T> From(ICollection<T> collection)
        {
            return new ListTag<T>(collection);
        }

    }

}
