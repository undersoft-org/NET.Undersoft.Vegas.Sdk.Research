namespace System.Collections.Generic
{   
    public static class SortedDictionaryExtension
    {
        public static void AddRange<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
            }
        }
        public static void AddDictionary<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
            }
        }
        public static SortedDictionary<T, S> AddRangeLog<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            SortedDictionary<T, S> result = new SortedDictionary<T, S>();
            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                    source.Add(item.Key, item.Value);
                else
                    result.Add(item.Key, item.Value);
            }
            return result;
        }
        public static void PutRange<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
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
        public static SortedDictionary<T, S> GetDictionary<T, S>(this SortedDictionary<T, S> source, T key)
        {
            if (key == null)
                throw new ArgumentNullException("Collection is null");
            SortedDictionary<T, S> result = new SortedDictionary<T, S>();
            if (source.ContainsKey(key))
                result.Add(key, source[key]);

            return result;
        }
        public static S Get<T, S>(this SortedDictionary<T, S> source, T key)
        {
            if (key == null)
                throw new ArgumentNullException("Collection is null");
            S result = default(S);
            if (source.ContainsKey(key))
                result = source[key];

            return result;
        }
        public static bool TryAdd<T, S>(this SortedDictionary<T, S> source, T Key, S Value)
        {
            if (Key == null || Value == null)
                throw new ArgumentNullException("Collection is null");

            if (source.ContainsKey(Key))
                return false;
            else
                source.Add(Key, Value);
            return true;
        }
        public static S AddOrUpdate<T, S>(this SortedDictionary<T, S> source, T Key, S Value, Func<T, S, S> func)
        {
            if (Key == null || Value == null)
                throw new ArgumentNullException("Collection is null");

            if (source.ContainsKey(Key))
                return source[Key] = func(Key, Value);
            else
            {
                source.Add(Key, Value);
                return Value;
            }

        }
        public static bool TryRemove<T, S>(this SortedDictionary<T, S> source, T Key, out S Value)
        {
            if (Key == null)
                throw new ArgumentNullException("Collection is null");

            if (source.TryGetValue(Key, out Value))
            {
                source.Remove(Key);
                return true;
            }
            return false;
        }
        public static void AddOrUpdateRange<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                source.AddOrUpdate(item.Key, item.Value, (k, v) => v = item.Value);
            }
        }
        public static void Put<T, S>(this SortedDictionary<T, S> source, T key, S item)
        {
            if (key == null || item == null)
                throw new ArgumentNullException("Collection is null");

            if (!source.ContainsKey(key))
                source.Add(key, item);
            else
                source[key] = item;

        }
        public static void Put<T, S>(this SortedDictionary<T, Dictionary<T, S>> source, T key, T key2, S item)
        {
            if (key == null || item == null)
                throw new ArgumentNullException("Collection is null");

            if (!source.ContainsKey(key))
                source.Add(key, new Dictionary<T, S>() { { key2, item } });
            else if (!source[key].ContainsKey(key2))
                source[key].Add(key2, item);
            else
                source[key][key2] = item;
        }
        public static void RemoveRange<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (source.ContainsKey(item.Key))
                    source.Remove(item.Key);
            }
        }
        public static void RemoveRange<T, S>(this SortedDictionary<T, S> source, IList<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");

            foreach (var item in collection)
            {
                if (source.ContainsKey(item))
                    source.Remove(item);
            }
        }
        public static List<S> GetRange<T, S>(this SortedDictionary<T, S> source, IList<T> collection)
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
        public static SortedDictionary<T, S> GetRange<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            SortedDictionary<T, S> result = new SortedDictionary<T, S>();
            foreach (var item in collection)
            {
                if (source.ContainsKey(item.Key))
                    result.Add(item.Key, source[item.Key]);
            }
            return result;
        }
        public static SortedDictionary<T, S>[] GetRangeLog<T, S>(this SortedDictionary<T, S> source, IDictionary<T, S> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("Collection is null");
            SortedDictionary<T, S>[] result = new SortedDictionary<T, S>[2];
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
