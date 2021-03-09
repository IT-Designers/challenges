using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SubmissionEvaluation.Contracts.Data
{
    public class StatsList<T> : IDictionary<string, T> where T : class
    {
        protected List<KeyValuePair<string, T>> list = new List<KeyValuePair<string, T>>();

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, T> item)
        {
            if (item.Key == null)
            {
                throw new ArgumentNullException(nameof(item.Key));
            }

            list.RemoveAll(x => x.Key == item.Key);
            list.Add(item);
        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return list.Any(x => x.Key == item.Key);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            var count = list.RemoveAll(x => x.Key == item.Key && x.Value == item.Value);
            return count > 0;
        }

        public int Count => list.Count;
        public bool IsReadOnly => false;

        public bool ContainsKey(string key)
        {
            return list.Any(x => x.Key == key);
        }

        public void Add(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            list.RemoveAll(x => x.Key == key);
            list.Add(new KeyValuePair<string, T>(key, value));
        }

        public bool Remove(string key)
        {
            var count = list.RemoveAll(x => x.Key == key);
            return count > 0;
        }

        public bool TryGetValue(string key, out T value)
        {
            var kp = list.FirstOrDefault(x => x.Key == key);
            value = kp.Value;
            return kp.Value != null || kp.Key == key;
        }

        public T this[string key]
        {
            get { return list.FirstOrDefault(x => x.Key == key).Value; }
            set
            {
                list.RemoveAll(x => x.Key == key);
                Add(key, value);
            }
        }

        public ICollection<string> Keys => list.Select(x => x.Key).ToList();
        public ICollection<T> Values => list.Select(x => x.Value).ToList();
    }
}
