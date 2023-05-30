using System;
using System.Collections.Generic;
using System.Text;

namespace VTNET.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// <para>Will rely on "separation condition" to group from "first element that meets the condition" to "the next element that meets the condition"</para>
        /// <para>For example: {1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0}</para>
        /// <para>Separation conditions: SplitGroup(x => x == 1)</para>
        /// <para>After separating:</para>
        /// <para>List 1: {1, 0, 0, 0, 0}</para>
        /// <para>List 2: {1, 0, 0, 0}</para>
        /// <para>List 3: {1, 0, 0}</para>
        /// <para>List 4: {1, 0}</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<List<T>> SplitGroup<T, U>(this IEnumerable<T> list, Func<T, U> selector)
        {
            List<List<T>> result = new List<List<T>>();
            List<T> sublist = new List<T>();
            U defaultValue = default;
            foreach (T item in list)
            {
                U fieldValue = selector(item);
                if (!fieldValue!.Equals(defaultValue) && sublist.Count > 0)
                {
                    result.Add(sublist);
                    sublist = new List<T>();
                }
                sublist.Add(item);
            }
            if (sublist.Count > 0)
            {
                result.Add(sublist);
            }
            return result;
        }

        /// <summary>
        /// Split a list into multiple lists
        /// <para>For example: {1, 2, 2, 2, 2, 1, 3, 3, 3, 1, 4, 4, 1, 5}</para>
        /// <para>Separation conditions: SplitGroup(x => x == 1)</para>
        /// <para>After separating:</para>
        /// <para>List 1: {2, 2, 2, 2}</para>
        /// <para>List 2: {3, 3, 3}</para>
        /// <para>List 3: {4, 4}</para>
        /// <para>List 4: {5}</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<List<T>> Split<T>(this List<T> source, Func<T, string> selector)
        {
            var result = new List<List<T>>();
            var tempDict = new Dictionary<string, List<T>>();

            foreach (var item in source)
            {
                var key = selector(item);
                if (!tempDict.ContainsKey(key))
                {
                    tempDict[key] = new List<T>();
                }

                tempDict[key].Add(item);
            }

            result.AddRange(tempDict.Values);

            return result;
        }

    }
}
