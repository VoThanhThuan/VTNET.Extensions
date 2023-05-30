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
        public static float FloatDouble(this string value)
        {
            return float.TryParse(value, out var result) ? result : float.NaN;
        }
        public static double ParseDouble(this string value)
        {
            return double.TryParse(value, out var result) ? result : double.NaN;
        }
        
    }
}
