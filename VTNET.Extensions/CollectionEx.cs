using System;
using System.Collections.Generic;

namespace VTNET.Extensions
{
    public static class CollectionEx
    {
        /// <summary>
        /// <para>Will rely on "separation condition" to group from "first element that meets the condition" to "the next element that meets the condition"</para>
        /// <para>For example: {1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0}</para>
        /// <para>Separation conditions: Split(x => x == 1)</para>
        /// <para>After separating:</para>
        /// <para>List 1: {1, 0, 0, 0, 0}</para>
        /// <para>List 2: {1, 0, 0, 0}</para>
        /// <para>List 3: {1, 0, 0}</para>
        /// <para>List 4: {1, 0}</para>
        /// </summary>
        /// <param name="list"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static List<List<T>> Split<T, U>(this IEnumerable<T> list, Func<T, U> selector)
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
        /// <para>Will rely on "separation condition" to group from "first element that meets the condition" to "the next element that meets the condition"</para>
        /// <para>For example: {1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0}</para>
        /// <para>Separation conditions: Split(1)</para>
        /// <para>After separating:</para>
        /// <para>List 1: {1, 0, 0, 0, 0}</para>
        /// <para>List 2: {1, 0, 0, 0}</para>
        /// <para>List 3: {1, 0, 0}</para>
        /// <para>List 4: {1, 0}</para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        public static List<List<T>> Split<T>(this List<T> list, T selector)
        {
            return list.Split(selector: x => x.Equals(selector));
        }

    }
}
