﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VTNET.Extensions
{
    public static class DatatableEx
    {
        public static List<T> ToList<T>(this DataTable table, bool matchCase = false) where T : new()
        {
            var properties = typeof(T).GetProperties()
                .ToDictionary(GetColumnName, prop => prop.Name, matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

            var list = table.AsEnumerable().Select(row =>
            {
                var item = new T();
                foreach (DataColumn column in table.Columns)
                {
                    if (properties.TryGetValue(column.ColumnName, out var propertyName) && row[column] != DBNull.Value)
                    {
                        var propertyInfo = typeof(T).GetProperty(propertyName);
                        propertyInfo?.SetValue(item, Convert.ChangeType(row[column], propertyInfo.PropertyType));
                    }
                }
                return item;
            }).ToList();
            return list;
        }
        public static List<T> ToListParallel<T>(this DataTable table) where T : new()
        {
            var list = new List<T>();
            var properties = typeof(T).GetProperties()
                .ToDictionary(prop => prop.GetCustomAttribute<ColumnNameAttribute>()?.Name ?? prop.Name, prop => prop.Name);

            _ = Parallel.ForEach(table.Rows.Cast<DataRow>(), row =>
            {
                var item = new T();
                foreach (DataColumn column in table.Columns)
                {
                    if (properties.TryGetValue(column.ColumnName, out var value) && row[column] != DBNull.Value)
                    {
                        var propertyInfo = typeof(T).GetProperty(value);
                        propertyInfo?.SetValue(item, Convert.ChangeType(row[column], propertyInfo.PropertyType), null);
                    }
                }
                lock (list)
                {
                    list.Add(item);
                }
            });

            return list;
        }
        public static List<T> ToListCache<T>(this DataTable table, bool matchCase = false) where T : new()
        {
            var properties = GetCachedProperties<T>(matchCase);

            var list = table.AsEnumerable().Select(row =>
            {
                var item = new T();

                foreach (DataColumn column in table.Columns)
                {
                    if (properties.TryGetValue(column.ColumnName, out var property) && row[column] != DBNull.Value)
                    {
                        var propertyInfo = properties[property.Property.Name];
                        propertyInfo.Setter(item, Convert.ChangeType(row[column], property.Property.PropertyType));
                    }
                }

                return item;
            }).ToList();

            return list;
        }

        private static Dictionary<string, PropertyInfoWrapper> GetCachedProperties<T>(bool matchCase)
        {
            var key = typeof(T).FullName + "_" + matchCase;

            if (PropertyCache.ContainsKey(key))
            {
                return PropertyCache[key];
            }
            else
            {
                var properties = typeof(T).GetProperties()
                    .ToDictionary(GetColumnName, prop => new PropertyInfoWrapper(prop), matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

                PropertyCache[key] = properties;
                return properties;
            }
        }
        private static string GetColumnName(PropertyInfo prop)
        {
            var columnNameAttribute = prop.GetCustomAttribute<ColumnNameAttribute>();
            return columnNameAttribute?.Name ?? prop.Name;
        }

        private class PropertyInfoWrapper
        {
            public PropertyInfoWrapper(PropertyInfo propertyInfo)
            {
                Property = propertyInfo;
                Setter = CreateSetterDelegate(propertyInfo);
            }

            public PropertyInfo Property { get; }
            public Action<object, object> Setter { get; }

            private static Action<object, object> CreateSetterDelegate(PropertyInfo propertyInfo)
            {
                var instance = Expression.Parameter(typeof(object), "instance");
                var value = Expression.Parameter(typeof(object), "value");

                var castInstance = Expression.Convert(instance, propertyInfo.DeclaringType);
                var castValue = Expression.Convert(value, propertyInfo.PropertyType);
                var property = Expression.Property(castInstance, propertyInfo);
                var assign = Expression.Assign(property, castValue);

                var lambda = Expression.Lambda<Action<object, object>>(assign, instance, value);

                return lambda.Compile();
            }
        }

        private static readonly Dictionary<string, Dictionary<string, PropertyInfoWrapper>> PropertyCache = new Dictionary<string, Dictionary<string, PropertyInfoWrapper>>();

    }
}
