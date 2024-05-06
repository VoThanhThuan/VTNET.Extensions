using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace VTNET.Extensions
{
    public static class NumberEx
    {
        static readonly Type[] _numberTypes = new Type[]
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
        
        public static bool ParseDecimal([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out decimal? result)
        {
            if (decimal.TryParse(s, style, provider, out var res))
            {
                result = res;
                return true;
            }
            result = default; 
            return false;
        }

        public static readonly decimal PI = 3.14159265358979323846m;

        public static decimal? Log10(decimal? m)
        {
            if(!m.HasValue)
                return 0;
            return (decimal)Math.Log10((double)m);
        }
        public static decimal? Sin(decimal? m)
        {
            if (!m.HasValue)
                return 0;
            return (decimal)Math.Sin((double)m);
        }
        public static decimal? Cos(decimal? m)
        {
            if (!m.HasValue)
                return 0;
            return (decimal)Math.Cos((double)m);
        }
        public static decimal? Tan(decimal? m)
        {
            if (!m.HasValue)
                return 0;
            return (decimal)Math.Tan((double)m);
        }
        public static decimal? Sqrt(decimal? m)
        {
            if (!m.HasValue)
                return 0;
            return (decimal)Math.Sqrt((double)m);
        }
        public static decimal? Abs(decimal? m)
        {
            if (!m.HasValue)
                return 0;
            return (decimal)Math.Abs((double)m);
        }
        public static decimal? Pow(decimal? a, decimal? b)
        {
            if (!a.HasValue || !b.HasValue)
                return 0;
            return (decimal)Math.Pow((double)a, (double)b);
        }
    }
}
