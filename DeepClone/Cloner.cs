using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace DeepClone
{
    public static class Cloner
    {
        #region deep clone
        static void UnSupportClone<T>(T value) where T : new()
        {
            if(value is DataTable) 
            {
                throw new Exception("Type not support clone!");
            }
        }
        public static T DeepClone<T>(T value) where T : new()
        {
            UnSupportClone(value);
            if (value is null) return default!;
            var newT = new T();
            if(value is IEnumerable enumerable1)
            {
                return (T)CloneList(enumerable1);
            }
            var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var propValue = prop.GetValue(value);
                    if (propValue is IEnumerable enumerable && !(propValue is string))
                    {
                        var clonedList = CloneList(enumerable);
                        prop.SetValue(newT, clonedList);
                    }
                    else
                    {
                        prop.SetValue(newT, propValue);
                    }
                }
            }
            return newT;
        }
        private static object? CloneList(IEnumerable original)
        {
            var listType = original.GetType();
            var elementType = listType.IsGenericType
                ? listType.GetGenericArguments()[0]
                : typeof(object);

            var newList = (IList?)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            foreach (var item in original)
            {
                var clonedItem = CloneItem(item);
                newList?.Add(clonedItem);
            }

            return newList;
        }
        private static object? CloneItem(object item)
        {
            if (item is ICloneable cloneable)
            {
                return cloneable.Clone();
            }

            var itemType = item.GetType();
            if (itemType.IsValueType || itemType == typeof(string))
            {
                return item;
            }

            var cloneMethod = typeof(Cloner).GetMethod(nameof(DeepClone))?.MakeGenericMethod(itemType);
            return cloneMethod?.Invoke(null, new[] { item });
        }
        #endregion deep clone
    }
}
