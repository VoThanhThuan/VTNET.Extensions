using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace VTNET.Extensions
{
    public static class DatatableEx
    {
        public static T Cast<T>(this DataRow row) where T : new()
        {
            var data = new T();
            var type = typeof(T);
            var propertiesInfo = type.GetProperties().Where(prop => prop.GetCustomAttribute<IgnoreMapColumnNameAttribute>() == null);
            foreach (var property in propertiesInfo)
            {
                var attribute = (MapColumnNameAttribute?)property.GetCustomAttribute(typeof(MapColumnNameAttribute));
                var propertyName = attribute?.Name ?? property.Name;
                var value = row[propertyName];
                if (value != DBNull.Value)
                {
                    if (property.PropertyType.IsAssignableFrom(value.GetType()))
                    {
                        property.SetValue(data, value, null);
                    }
                    else
                    {
                        Type targetType = property.PropertyType;
                        try
                        {
                            object convertedValue = Convert.ChangeType(value, targetType);
                            property.SetValue(data, convertedValue, null);
                        }
                        catch (Exception)
                        {
                            // Handle the case where conversion is not possible
                            if (Nullable.GetUnderlyingType(targetType) != null)
                            {
                                property.SetValue(data, null, null);
                            }
                            else
                            {
                                // Handle the case where conversion is not possible and the property is not nullable
                                throw new InvalidOperationException($"Cannot convert data of type \"{value.GetType().Name}\" for  \"{value}\" into data of type \"{property.PropertyType.Name}\" for property '\"{typeof(T).FullName}.{property.Name}\".");
                            }
                        }
                    }
                }
                else if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                {
                    property.SetValue(data, null, null);
                }

            }
            return data;
        }
        static string GetColumnName(PropertyInfo prop)
        {
            var columnNameAttribute = prop.GetCustomAttribute<MapColumnNameAttribute>();
            return columnNameAttribute?.Name ?? prop.Name;
        }

        static Dictionary<string, string> GetProperties<T>(bool matchCase = false)
        {
            var properties = typeof(T).GetProperties()
                .Where(prop => prop.GetCustomAttribute<IgnoreMapColumnNameAttribute>() == null)
                .ToDictionary(GetColumnName, prop => prop.Name, matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
            return properties;
        }

        /// <summary>
        /// Fast data type conversion, bypassing checks (vague error messages), in exchange for a faster speed for data type conversion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CastFast<T>(this DataRow row) where T : new()
        {
            var data = new T();
            var type = typeof(T);
            var propertiesInfo = type.GetProperties().Where(prop => prop.GetCustomAttribute<IgnoreMapColumnNameAttribute>() == null);
            foreach (var property in propertiesInfo)
            {
                var propertyName = GetColumnName(property);
                var value = row[propertyName];
                var propertyInfo = typeof(T).GetProperty(propertyName);
                if (propertyInfo is null) continue;
                if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                    if (underlyingType is null)
                    {
                        propertyInfo.SetValue(data, value);
                    }
                    else
                    {
                        propertyInfo.SetValue(data, Convert.ChangeType(value, underlyingType));
                    }
                }
                else
                {
                    propertyInfo?.SetValue(data, Convert.ChangeType(value, propertyInfo.PropertyType));
                }
            }
            return data;
        }

        static void Map<T>(DataTable table, Dictionary<string, string> properties, T item, DataRow row)
        {
            foreach (DataColumn column in table.Columns)
            {
                if (properties.TryGetValue(column.ColumnName, out var propertyName) && row[column] != DBNull.Value)
                {
                    var propertyInfo = typeof(T).GetProperty(propertyName);
                    if (propertyInfo is null) continue;
                    if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                        if(underlyingType is null)
                        {
                            propertyInfo.SetValue(item, row[column]);
                        }
                        else
                        {
                            propertyInfo.SetValue(item, Convert.ChangeType(row[column], underlyingType));
                        }
                    }
                    else
                    {
                        propertyInfo?.SetValue(item, Convert.ChangeType(row[column], propertyInfo.PropertyType));
                    }
                }
            }
        }

        public static List<T> ToList<T>(this DataTable table, bool matchCase = false) where T : new()
        {
            var properties = GetProperties<T>(matchCase);
            var list = table.AsEnumerable().Select(row =>
            {
                var item = new T();
                Map(table, properties, item, row);
                return item;
            }).ToList();
            return list;
        }

        public static List<T?> ToListWithActivator<T>(this DataTable table, bool matchCase = false)
        {
            var properties = GetProperties<T>(matchCase);

            var list = table.AsEnumerable().Select(row =>
            {
                var item = (T?)Activator.CreateInstance(typeof(T));
                Map(table, properties, item, row);
                return item;
            }).ToList();
            return list;
        }

        public static List<T> ToListParallel<T>(this DataTable table, bool matchCase = false) where T : new()
        {
            var list = new List<T>();
            var properties = GetProperties<T>(matchCase);

            _ = Parallel.ForEach(table.Rows.Cast<DataRow>(), row =>
            {
                var item = new T();
                Map(table, properties, item, row);
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
                        var propertyInfo = properties[column.ColumnName];
                        if (propertyInfo.Property.PropertyType.IsGenericType && propertyInfo.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type underlyingType = Nullable.GetUnderlyingType(propertyInfo.Property.PropertyType);
                            propertyInfo.Setter(item, Convert.ChangeType(row[column], underlyingType));
                        }
                        else
                        {
                            propertyInfo?.Setter(item, Convert.ChangeType(row[column], propertyInfo.Property.PropertyType));
                        }

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
                    .Where(prop => prop.GetCustomAttribute<IgnoreMapColumnNameAttribute>() == null)
                    .ToDictionary(GetColumnName, prop => new PropertyInfoWrapper(prop), matchCase ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

                PropertyCache[key] = properties;
                return properties;
            }
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

        public static List<Dictionary<string, object>> ToListDictionary(this DataTable dataTable, Func<string, string> formatName)
        {
            //var dictionary = new Dictionary<string, object>();

            var dataList = new List<Dictionary<string, object>>();
            foreach (DataRow row in dataTable.Rows)
            {
                var data = new Dictionary<string, object>();
                foreach (DataColumn col in dataTable.Columns)
                {
                    data[col.ColumnName] = row[col];
                }
                dataList.Add(data);
            }
            return dataList;
        }

        public static DataTable SortTable(this DataTable table, string columnsName, bool ascending = true)
        {
            return SortTable(table, new List<string>() { columnsName}, ascending);
        }
        public static DataTable SortTable(this DataTable table, List<string> columnsName, bool ascending = true)
        {
            var dataView = table.DefaultView;
            var sortBy = ascending ? " ASC" : " DESC";
            dataView.Sort = string.Join($" {sortBy},", columnsName) + $" {sortBy}";
            return dataView.ToTable();
        }

        public static DataTable ToDataTable(this List<Dictionary<string, object?>> data)
        {
            var dataTable = new DataTable();
            foreach (var key in data[0].Keys)
            {
                dataTable.Columns.Add(key, typeof(object));
            }
            foreach (var dict in data)
            {
                DataRow row = dataTable.NewRow();
                foreach (var key in dict.Keys)
                {
                    row[key] = dict[key];
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static object? GetValue(this DataRow row, string ColumnName)
        {
            var data = row[ColumnName];
            return GetValue<object>(data);
        }
        public static T? GetValue<T>(this DataRow row, string ColumnName)
        {
            var data = row[ColumnName];
            return GetValue<T>(data);
        }
        public static T? GetValue<T>(object? data)
        {
            return data is null || data is DBNull ? default : (T)data;
        }
        public static void SetValue(this DataRow row, string ColumnName, object? data)
        {
            row[ColumnName] = GetDbValue(data);
        }
        public static object GetDbValue(object? data)
        {
            return data is null ? DBNull.Value : data;
        }

    }
}
