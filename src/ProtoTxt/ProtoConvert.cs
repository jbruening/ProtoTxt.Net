using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ProtoTxt
{
    /// <summary>
    /// Converter for ProtoTxt files
    /// </summary>
    public class ProtoConvert
    {
        /// <summary>
        /// Deserialize a type T from content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string content)
        {
            var tokens = Parser.Tokenize(ref content);
            var proto = Lexer.Lex(tokens, ref content);

            var obj = Activator.CreateInstance(typeof (T));
            DeserializeObject(obj, proto);

            return (T)obj;
        }

        private static object DeserializeProperty(ProtoProp pprop, PropertyInfo oprop)
        {
            if (!(pprop.Value is ProtoObject)) return pprop.Value;

            var ptype = oprop.PropertyType;
            if (ptype.IsGenericType)
                ptype = ptype.GetGenericArguments()[0];

            var pobj = pprop.Value as ProtoObject;
            var obj = Activator.CreateInstance(ptype);

            DeserializeObject(obj, pobj);

            return obj;
        }

        private static void DeserializeObject(object obj, ProtoObject proto)
        {
            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var pprop in proto.Properties)
            {
                var oprop = GetBestMatchingProperty(props, pprop.Name);
                if (oprop == null) continue;
                var value = DeserializeProperty(pprop, oprop);
                if (value == null) continue;
                var ptype = oprop.PropertyType;
                if (!ptype.IsGenericType)
                    oprop.SetValue(obj, ConvertValue(value, ptype));
                //todo: handle array properties similarly to List properties
                else
                {
                    var gtype = typeof (ICollection<>);
                    var args = ptype.GetGenericArguments();
                    var ctype = gtype.MakeGenericType(args);
                    if (ctype.IsAssignableFrom(ptype))
                    {
                        var existing = oprop.GetValue(obj);
                        if (existing == null)
                        {
                            existing = Activator.CreateInstance(ptype);
                            oprop.SetValue(obj, existing);
                        }
                        var add = ptype.GetMethod("Add");
                        if (add == null)
                            throw new Exception("Could not get Add method on type " + ptype);
                        var parm = add.GetParameters()[0];
                        add.Invoke(existing, new[] {ConvertValue(value, parm.ParameterType)});
                    }
                    else
                    {
                        throw new Exception("Unexpected type " + ptype);
                    }
                }
            }
        }

        private static PropertyInfo GetBestMatchingProperty(PropertyInfo[] props, string propertyName)
        {
            //with the proto generate, most of the time the properties have _ removed, and their name changed to PascalCase
            return FindProperty(props, propertyName.Replace("_", ""))
                ?? FindProperty(props, propertyName); //sometimes it's not modified
        }

        static PropertyInfo FindProperty(PropertyInfo[] props, string propertyName)
        {
            return props.FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));
        }

        private static object ConvertValue(object value, Type ptype)
        {
            if (ptype.IsInstanceOfType(value)) return value;
            
            if (ptype.IsEnum && value is string)
                value = Enum.Parse(ptype, value as string, true);
            else
                value = Convert.ChangeType(value, ptype);
            return value;
        }
    }
}
