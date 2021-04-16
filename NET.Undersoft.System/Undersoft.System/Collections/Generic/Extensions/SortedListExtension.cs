using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;

namespace System.Collections.Generic
{
    public static class SortedListExtension
    {
        public static void AddRange<T, S>(this SortedList<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
            }
        }      
        public static SortedList<T, S> AddRangeLog<T, S>(this SortedList<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            SortedList<T, S> result = new SortedList<T, S>();
            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
                else
                    result.Add(item.Key, item.Value);
            }
            return result;
        }     
        public static void PutRange<T, S>(this SortedList<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
                else
                    source[item.Key] = item.Value;
            }
        }      
        public static SortedList<T, S> GetDictionary<T, S>(this SortedList<T, S> source, T key)
        {
            if (key == null)
                throw new ArgumentNullException("Collection is null");
            SortedList<T, S> result = new SortedList<T, S>();
            if (source.ContainsKey(key))
                result.Add(key, source[key]);

            return result;
        }
        public static S Get<T, S>(this SortedList<T, S> source, T key)
        {
            if (key == null)
                throw new ArgumentNullException("Collection is null");
            S result = default(S);
            if (source.ContainsKey(key))
                result = source[key];

            return result;
        }
        public static void Put<T, S>(this SortedList<T, S> source, T key, S item)
        {           
            if (!source.ContainsKey(key))
                source.Add(key, item);
            else
                source[key] = item;
        }
        public static void RemoveRange<T, S>(this SortedList<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (source.ContainsKey(item.Key))
                    source.Remove(item.Key);
            }
        }
        public static void RemoveRange<T, S>(this SortedList<T, S> source, IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (source.ContainsKey(item))
                    source.Remove(item);
            }
        }
        public static List<S> GetRange<T, S>(this SortedList<T, S> source, IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            List<S> result = new List<S>();
            foreach (var item in collection)
            {
                if (source.ContainsKey(item))
                    result.Add(source[item]);
            }
            return result;
        }
        public static SortedList<T, S> GetRange<T, S>(this SortedList<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            SortedList<T, S> result = new SortedList<T, S>();
            foreach (var item in collection)
            {
                if (source.ContainsKey(item.Key))
                    result.Add(item.Key, source[item.Key]);
            }
            return result;
        }
        public static SortedList<T, S>[] GetRangeLog<T, S>(this SortedList<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            SortedList<T, S>[] result = new SortedList<T, S>[2];
            foreach (var item in collection)
            {
                if (source.ContainsKey(item.Key))
                    result[0].Add(item.Key, source[item.Key]);
                else
                    result[1].Add(item.Key, item.Value);
            }
            return result;
        }       
    }
}
