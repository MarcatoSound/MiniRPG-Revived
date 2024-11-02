using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BasicRPGTest_Mono.Engine.Data
{
    public class MiniSerializable
    {
        public string serialize()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            List<PropertyInfo> properties = (List<PropertyInfo>)this.GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SavedProperty)));
            PropertyInfo last = properties[properties.Count - 1];

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute(typeof(SavedProperty)) == null) continue;

                if (property.GetType() == typeof(MiniSerializable))
                {

                    sb.Append(((MiniSerializable)property.GetValue(this)).serialize());

                } else if (property.GetType() == typeof(ICollection<Object>))
                {

                    ICollection<Object> iterable = (ICollection<Object>)property.GetValue(this);
                    Type type = iterable.GetType().GetGenericArguments()[0];
                    if (type == typeof(MiniSerializable))
                    {
                        sb.Append("[");
                        MiniSerializable miniLast = (MiniSerializable)iterable.Last();
                        foreach (MiniSerializable obj in iterable)
                        {
                            sb.Append(obj.serialize());
                            if (obj != miniLast) sb.Append("|");
                        }
                        sb.Append("]");
                    }

                } else
                {

                    sb.Append(property.GetValue(this));

                }
                if (property != last) sb.Append(",");
            }

            sb.Append("}");

            return sb.ToString();
        }

        /*public static T deserialize<T>(string data)
        {
            List<PropertyInfo> properties = (List<PropertyInfo>)typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(SavedProperty)));

            data.Split("|");
        }*/
    }

    public class SavedProperty : Attribute
    {

    }
}
