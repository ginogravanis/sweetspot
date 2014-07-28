using System;
using System.Reflection;

namespace SweetspotApp.Util
{
    public static class StringEnum
    {
        public static string GetStringValue(Enum value)
        {
            Type type = value.GetType();
            FieldInfo fi = type.GetField(value.ToString());
            StringValueAttribute[] attributes =
                fi.GetCustomAttributes(typeof (StringValueAttribute), false)
                as StringValueAttribute[];
            if (attributes.Length > 0)
            {
                return attributes[0].Value;
            }
            else
            {
                throw new InvalidOperationException("Element has no StringValueAttribute.");
            }
        }
    }
}
