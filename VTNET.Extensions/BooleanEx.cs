using System;
using System.Collections;

namespace VTNET.Extensions
{
    public static class BooleanEx
    {
        /// <summary>
        /// True if value is greater than zero and non-empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsTrue(object? value)
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
            else if (NumberEx.IsNumberType(value))
            {
                var number = Convert.ToDouble(value);
                if (number < 1)
                {
                    return false;
                }
            }
            else if (value is Array array)
            {
                return array.Length > 0;
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
            return true;
        }

        /// <summary>
        /// False if value is less than zero and empty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsFalse(object? value)
        {
            return !IsTrue(value);
        }
    }
}
