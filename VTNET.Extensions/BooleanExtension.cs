using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace VTNET.Extensions
{
    public static class BooleanExtension
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
        /// True if value is greater than zero and non-empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(this object? value)
        {
            if (value is null)
            {
                return false;
            }

            if (value is bool boolean)
            {
                return boolean;
            }
            else if (value is string str)
            {
                if ((double.TryParse(str, out var num) && num < 1))
                {
                    return false;
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(str);
                }
            }
            else if (IsNumberType(value))
            {
                var number = Convert.ToDouble(value);
                if (number < 1)
                {
                    return false;
                }
            }
            else if (value is Array array)
            {
                return array.Length > 1;
            }
            else if (value is ICollection collection && collection.Count < 1)
            {
                return false;
            }
            else if (value is Enum && value.GetHashCode() == 0)
            {
                return false;
            }
            else if (value is Guid guid && guid == Guid.Empty)
            {
                return false;
            }
            else if (value is DateTime dt && dt == DateTime.MinValue)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// False if value is less than zero and empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalse(this object? value)
        {
            return !IsTrue(value);
        }
        /// <summary>
        /// Similar <see cref="string.IsNullOrEmpty(string?)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string? text)
        {
            return string.IsNullOrEmpty(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string? text)
        {
            return string.IsNullOrWhiteSpace(text);
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
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsNumericString(this string? input, char? thousandSeparator = null, char? decimalDigits = null)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (thousandSeparator != null)
            {
                input = input.Replace(thousandSeparator.ToString(), "");
            }
            if(decimalDigits != null)
            {
                input = input.Replace(decimalDigits.ToString(), ".");
            }
            string pattern = @"^-?\d*\.?\d+$";
            return Regex.IsMatch(input, pattern);
        }
    }
}
