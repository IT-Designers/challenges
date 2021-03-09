using System.Collections.Generic;
using System.IO;
using System.Linq;
using Force.DeepCloner;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Providers.FileProvider
{
    internal class CachedYamlProvider : YamlProvider
    {
        private const int cacheSize = 16000;
        private readonly object lockkey = new object();
        private readonly ILog log;
        private Dictionary<string, CacheEntry> cache = new Dictionary<string, CacheEntry>();

        public CachedYamlProvider(ILog log) : base(log)
        {
            this.log = log;
        }

        public override T Deserialize<T>(string pathToFile, HandleMode mode = HandleMode.ThrowException, bool forceLoad = false)
        {
            if (!forceLoad)
            {
                var entry = GetEntryFromCache<T>(pathToFile);
                if (entry != null)
                {
                    return entry;
                }
            }

            var newItem = base.Deserialize<T>(pathToFile, mode, forceLoad);
            AddEntryToCache(pathToFile, newItem);
            return newItem;
        }

        public override T DeserializeWithDescription<T>(string pathToFile, bool forceLoad = false)
        {
            var entry = GetEntryFromCache<T>(pathToFile);
            if (entry != null)
            {
                return entry;
            }

            var newItem = base.DeserializeWithDescription<T>(pathToFile);
            AddEntryToCache(pathToFile, newItem);
            return newItem;
        }

        public override IEnumerable<TestParameters> DeserializeTestProperties(string pathToFile)
        {
            var entry = GetEntryFromCache<IEnumerable<TestParameters>>(pathToFile);
            if (entry != null)
            {
                return entry;
            }

            var newItem = base.DeserializeTestProperties(pathToFile).ToList();
            AddEntryToCache(pathToFile, newItem);
            return newItem;
        }

        private void AddEntryToCache<T>(string pathToFile, T newItem)
        {
            lock (lockkey)
            {
                var newEntry = new CacheEntry {AccessCounter = 1, Content = newItem, FileDateTime = File.GetLastWriteTime(pathToFile), Path = pathToFile};

                if (cache.ContainsKey(pathToFile))
                {
                    cache.Remove(pathToFile);
                }

                cache.Add(pathToFile, newEntry);
            }
        }

        private T GetEntryFromCache<T>(string pathToFile) where T : class
        {
            lock (lockkey)
            {
                if (cache.Values.Count > cacheSize * 1.5)
                {
                    CleanCache();
                }

                if (File.Exists(pathToFile) && cache.TryGetValue(pathToFile, out var entry))
                {
                    if (entry.FileDateTime == File.GetLastWriteTime(entry.Path))
                    {
                        entry.AccessCounter++;
                        return (T) entry.Content.DeepClone();
                    }
                }

                return null;
            }
        }

        public override void Serialize<T>(string pathToFile, T obj, bool emitDefaults = true, bool referenceDuplicates = true)
        {
            lock (lockkey)
            {
                cache.Remove(pathToFile);
            }

            base.Serialize(pathToFile, obj, emitDefaults, referenceDuplicates);
        }

        public override void SerializeWithDescription<T>(string pathToFile, T obj)
        {
            lock (lockkey)
            {
                cache.Remove(pathToFile);
            }

            base.SerializeWithDescription(pathToFile, obj);
        }

        private void CleanCache()
        {
            lock (lockkey)
            {
                log.Warning("Räume Cache auf. Derzeit {count} Einträge", cache.Values.Count);
                var leftEntries = cache.Values.OrderByDescending(x => x.AccessCounter).Take(cacheSize);
                cache = new Dictionary<string, CacheEntry>();
                foreach (var entry in leftEntries)
                {
                    cache[entry.Path] = entry;
                }
            }
        }
    }
}
