using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace VTNET.Extensions
{
    public static class BooleanEx
    {
        /// <summary>
        /// True if value is greater than zero and non-empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue([NotNullWhen(true)] object? value)
        {
            if (value is null)
            {
                return false;
            }

            if (value is bool boolean)
            {
                return boolean;
            }
            if (value is string str)
            {
                if (str.IsNullOrWhiteSpace())
                {
                    return false;
                }else{
                    return ((double.TryParse(str, out var num) && num < 1));
                }
            }
            if (NumberEx.IsNumberType(value))
            {
                var number = Convert.ToDouble(value);
                if (number < 1)
                {
                    return false;
                }
            }
            if (value is Array array)
            {
                return array.Length > 0;
            }
            if (value is ICollection collection && collection.Count < 1)
            {
                return false;
            }
            if (value is Enum && value.GetHashCode() == 0)
            {
                return false;
            }
            if (value is Guid guid && guid == Guid.Empty)
            {
                return false;
            }
            if (value is DBNull)
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
        public static bool IsFalse([NotNullWhen(false)] object? value)
        {
            return !IsTrue(value);
        }
    }
}
