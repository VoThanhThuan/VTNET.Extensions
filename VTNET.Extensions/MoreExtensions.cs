using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using VTNET.Extensions.Languages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;

namespace VTNET.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class MoreExtension
    {
        /// <summary>
        /// Đây là Console.WriteLine()
        /// </summary>
        /// <param name="text"></param>
        public static void Log(this object obj)
        {
            Console.WriteLine(obj);
        }
        
        public static void Log(this object obj, params object[] objArray)
        {
            var str = new StringBuilder();
            str.Append(obj);
            foreach (var item in objArray)
            {
                str.Append(item);
            }
            Console.WriteLine(str);
        }
    }
}