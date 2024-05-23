using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using VTNET.Extensions.Models;
using System.Linq.Expressions;

namespace VTNET.Extensions;
public static class ClassEx
{
    public static T Map<T>(this object obj) where T : new()
    {
        // Tạo một đối tượng mới của kiểu T
        T newObj = new();
        var a = new List<T>();
        var b = a.Count();
        // Lấy thông tin về các thuộc tính của obj
        //var propertiesT = GetProperties<T>();
        var propertiesT = GetProperties<T>();
        PropertyInfo[] properties = obj.GetType().GetProperties();

        // Sao chép giá trị của mỗi thuộc tính từ obj sang newObj
        foreach (var property in properties)
        {
            var attribute = (MapNameAttribute?)property.GetCustomAttribute(typeof(MapNameAttribute));
            var propertyName = attribute?.Name ?? property.Name;

            if(propertiesT.TryGetValue(propertyName, out var propertyInfoT))
            {
                // Kiểm tra xem thuộc tính có thể gán giá trị không
                if (propertyInfoT.CanWrite)
                {
                    // Lấy giá trị của thuộc tính từ obj
                    object value = property.GetValue(obj)!;

                    // Gán giá trị cho thuộc tính tương ứng trên newObj
                    propertyInfoT.SetValue(newObj, value);
                }
            }
        }

        return newObj;
    }
    internal static string GetColumnName(PropertyInfo prop)
    {
        var columnNameAttribute = prop.GetCustomAttribute<MapNameAttribute>();
        return columnNameAttribute?.Name ?? prop.Name;
    }
    internal static Dictionary<string, PropertyInfo> GetProperties<T>(bool matchCase = false)
    {
        var properties = typeof(T).GetProperties()
            .Where(prop => prop.GetCustomAttribute<IgnoreMapNameAttribute>() == null)
            .ToDictionary(GetColumnName, prop => prop, matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        return properties;
    }
    internal static Dictionary<string, PropertyInfo> GetProperties(object obj, bool matchCase = false)
    {
        var properties = obj.GetType().GetProperties()
            .Where(prop => prop.GetCustomAttribute<IgnoreMapNameAttribute>() == null)
            .ToDictionary(GetColumnName, prop => prop, matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        return properties;
    }

    public static ValidatorFields<T> Validation<T>(T obj)
    {
        return new(obj);
    }

}


