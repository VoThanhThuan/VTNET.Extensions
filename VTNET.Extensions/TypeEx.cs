using System.Collections.Generic;
using System;

namespace VTNET.Extensions;
public static class TypeEx
{
    public static Dictionary<Type, object> CustomDefaultValue { get; set; } = new();
    static Dictionary<Type, object> _defaultValues = new()
    {
            { typeof(bool), default(bool) },
            { typeof(byte), default(byte) },
            { typeof(sbyte), default(sbyte) },
            { typeof(char), default(char) },
            { typeof(decimal), default(decimal) },
            { typeof(double), default(double) },
            { typeof(float), default(float) },
            { typeof(int), default(int) },
            { typeof(uint), default(uint) },
            { typeof(long), default(long) },
            { typeof(ulong), default(ulong) },
            { typeof(short), default(short) },
            { typeof(ushort), default(ushort) },
            { typeof(string), "" },
            { typeof(object), "" },
            { typeof(DateTime), DateTime.Now },
            { typeof(Guid), default(Guid) },
            { typeof(TimeSpan), DateTime.Now.TimeOfDay },
            { typeof(byte[]), Array.Empty<byte>() },
        };
    public static object GetDefaultValue(this Type type)
    {
        return _defaultValues[type];
    }
    public static object? GetCustomDefaultValue(Type type)
    {
        if (_defaultValues.TryGetValue(type, out var value))
        {
            return value;
        }
        else if(CustomDefaultValue.TryGetValue(type, out var cValue))
        {
            return cValue;
        }
        return null;
    }

}
