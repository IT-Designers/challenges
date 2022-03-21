using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SubmissionEvaluation.Contracts.Data
{
    public class StatsList<T> : IDictionary<string, T> where T : class
    {
        protected List<KeyValuePair<string, T>> List = new List<KeyValuePair<string, T>>();

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return List.GetEnumerator();
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

            List.RemoveAll(x => x.Key == item.Key);
            List.Add(item);
        }

        public void Clear()
        {
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            return List.Any(x => x.Key == item.Key);
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            var count = List.RemoveAll(x => x.Key == item.Key && x.Value == item.Value);
            return count > 0;
        }

        public int Count => List.Count;
        public bool IsReadOnly => false;

        public bool ContainsKey(string key)
        {
            return List.Any(x => x.Key == key);
        }

        public void Add(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            List.RemoveAll(x => x.Key == key);
            List.Add(new KeyValuePair<string, T>(key, value));
        }

        public bool Remove(string key)
        {
            var count = List.RemoveAll(x => x.Key == key);
            return count > 0;
        }

        public bool TryGetValue(string key, out T value)
        {
            var kp = List.FirstOrDefault(x => x.Key == key);
            value = kp.Value;
            return kp.Value != null || kp.Key == key;
        }

        public T this[string key]
        {
            get { return List.FirstOrDefault(x => x.Key == key).Value; }
            set
            {
                List.RemoveAll(x => x.Key == key);
                Add(key, value);
            }
        }

        public ICollection<string> Keys => List.Select(x => x.Key).ToList();
        public ICollection<T> Values => List.Select(x => x.Value).ToList();
    }
}
