using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VTNET.Extensions
{
    public static class NumberExtension
    {
        /// <summary>
        /// Check if object is numeric?
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static bool IsNumberType(object variable)
        {
            return variable is sbyte || variable is byte ||
               variable is short || variable is ushort ||
               variable is int || variable is uint ||
               variable is long || variable is ulong ||
               variable is float || variable is double || variable is decimal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static bool IsEven(this object? number)
        {
            if (number != null && IsNumberType(number))
            {
                var numericValue = Convert.ToInt64(number);
                return numericValue % 2 == 0;
            }

            throw new InvalidOperationException($"Number of type '{number?.GetType()}' is not supported.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsOdd(this object? number)
        {
            return !number.IsEven();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static dynamic? ToDynamic(this object value)
        {
            IDictionary<string, object?> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }
        public static int ParseInt(this string value)
        {
            _ = double.TryParse(value, out var result);
            return (int)result;
        }
        public static float ParseFloat(this string value)
        {
            return float.TryParse(value, out var result) ? result : float.NaN;
        }
        public static double ParseDouble(this string value)
        {
            return double.TryParse(value, out var result) ? result : double.NaN;
        }
        
    }
}
