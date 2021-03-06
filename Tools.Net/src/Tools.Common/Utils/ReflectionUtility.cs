using System;
using System.Collections.Generic;
using System.Text;
using Tools.Common.Asserts;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace Tools.Common.Utils
{
    /// <summary>
    /// Provides reflection related utility methods.
    /// </summary>
    public static class ReflectionUtility
    {
        /// <summary>
        /// Dumps the first level of properties and fields. Is not 
        /// recursing through, only traverses one level.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DumpObjectFieldsAndProperties(object source)
        {
            ErrorTrap.AddRaisableAssertion<ArgumentNullException>(
                source != null, "Source object for dumping properties is " +
                "not expected to be null!");

            PropertyInfo[] propertyInfos = source.GetType().GetProperties();

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Properties:").Append(Environment.NewLine);

            foreach (PropertyInfo pi in propertyInfos)
            {
                object value = pi.GetValue(source, null);

                stringBuilder.AppendFormat("\t{0}:{1}", pi.Name,
                    (value == null) ? "null" : value).Append(Environment.NewLine);
            }

            FieldInfo[] fieldInfos = source.GetType().GetFields();

            stringBuilder.Append("Fields:").Append(Environment.NewLine);

            foreach (FieldInfo fi in fieldInfos)
            {
                object value = fi.GetValue(source);

                stringBuilder.AppendFormat("\t{0}:{1}", fi.Name,
                    (value == null) ? "null" : value).Append(Environment.NewLine);
            }

            return stringBuilder.ToString();

        }

        /// <summary>
        /// Gets the object's properties as a dictionary of property name-value.
        /// </summary>
        /// <param name="source">source object to parse to dictionary</param>
        /// <returns>dictionary of properties name-value</returns>
        public static IDictionary GetObjectPropertiesDictionary(object source)
        {
            IDictionary values = new Hashtable();

            if (source == null) return values;

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                values.Add(property.Name, property.GetValue(source));
            }

            return values;
        }
    }
}
