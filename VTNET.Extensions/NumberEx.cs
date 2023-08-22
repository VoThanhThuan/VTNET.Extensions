using System;
using System.Linq;

namespace VTNET.Extensions
{
    public static class NumberEx
    {
        static Type[] _numberTypes = new Type[]
        {
                typeof(sbyte), typeof(byte),
                typeof(short), typeof(ushort),
                typeof(int), typeof(uint),
                typeof(long), typeof(ulong),
                typeof(float), typeof(double), typeof(decimal)
        };
        /// <summary>
        /// Check if object is numeric?
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public static bool IsNumberType(object? variable)
        {
            return _numberTypes.Contains(variable?.GetType());
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

        public static double ParseNumber(this string value)
        {
            return double.TryParse(value, out var result) ? result : double.NaN;
        }
        
    }
}
