using System;

namespace VTNET.Extensions
{
    public static class NumberEx
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

        public static bool IsEven(object? number)
        {
            if (number != null && IsNumberType(number))
            {
                var numericValue = Convert.ToInt64(number);
                return numericValue % 2 == 0;
            }

            throw new InvalidOperationException($"Number of type '{number?.GetType()}' is not supported.");
        }

        public static bool IsOdd(object? number)
        {
            return !IsEven(number);
        }

        public static bool IsEven(this int number)
        {
            return number % 2 == 0;
        } 
        public static bool IsEven(this long number)
        {
            return number % 2 == 0;
        }
        public static bool IsEven(this float number)
        {
            return number % 2 == 0;
        }
        public static bool IsEven(this double number)
        {
            return number % 2 == 0;
        }        
        
        public static bool IsOdd(this int number)
        {
            return !number.IsEven();
        }
        public static bool IsOdd(this long number)
        {
            return !number.IsEven();
        }
        public static bool IsOdd(this float number)
        {
            return !number.IsEven();
        }
        public static bool IsOdd(this double number)
        {
            return !number.IsEven();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static dynamic? ToDynamic(this object value)
        //{
        //    IDictionary<string, object?> expando = new ExpandoObject();

        //    foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
        //        expando.Add(property.Name, property.GetValue(value));

        //    return expando as ExpandoObject;
        //}
        //public static int ParseInt(this string value)
        //{
        //    _ = double.TryParse(value, out var result);
        //    return (int)result;
        //}
        //public static float ParseFloat(this string value)
        //{
        //    return float.TryParse(value, out var result) ? result : float.NaN;
        //}
        public static double ParseNumber(this string value)
        {
            return double.TryParse(value, out var result) ? result : double.NaN;
        }
        
    }
}
