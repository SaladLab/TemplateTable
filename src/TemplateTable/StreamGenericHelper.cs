using System;
using System.IO;

namespace TemplateTable
{
    internal static class StreamGenericHelper
    {
        public static T Read<T>(BinaryReader reader)
        {
            if (typeof(T) == typeof(int))
                return (T)(object)reader.ReadInt32();
            else if (typeof(T) == typeof(string))
                return (T)(object)reader.ReadString();
            else
                throw new NotSupportedException();
        }

        public static void Write<T>(BinaryWriter writer, T value)
        {
            if (value is int)
                writer.Write((int)(object)value);
            else if (value is string)
                writer.Write((string)(object)value);
            else
                throw new NotSupportedException();
        }
    }
}
