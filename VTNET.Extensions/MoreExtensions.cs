using System;
using System.Text;

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