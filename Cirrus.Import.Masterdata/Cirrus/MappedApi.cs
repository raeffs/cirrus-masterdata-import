using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cirrus.Import.Masterdata.Cirrus
{
    abstract class MappedApi<TValue> : IDisposable
    {
        private readonly HashSet<Mapping<TValue>> knownMappings = new HashSet<Mapping<TValue>>();
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

        public async Task<string> GetMappingAsync(string key, TValue value)
        {
            return (await this.GetMappingsAsync(key, new[] { value })).Select(x => x.Id).SingleOrDefault();
        }

        public async Task<IEnumerable<Mapping<TValue>>> GetMappingsAsync(string key, IEnumerable<TValue> values)
        {
            values = values.Distinct().ToList();

            var mappings = this.GetMappings(key, values);
            var missing = values.Where(x => !mappings.Any(y => x.Equals(y.Value)));

            if (!missing.Any())
            {
                return mappings;
            }

            var loaded = await this.LoadMappingsAsync(key, missing);
            var duplicates = loaded.GroupBy(x => x).Where(x => x.Count() > 1).ToList();

            foreach (var duplicate in duplicates)
            {
                var toKeep = duplicate.Key;
                var toRemove = duplicate.Where(x => !ReferenceEquals(x, toKeep));
                loaded = loaded.Where(x => !toRemove.Any(y => ReferenceEquals(x, y)));
                await this.ProcessDuplicatesAsync(toKeep, toRemove);
            }

            this.AddMappings(loaded);

            return this.GetMappings(key, values);
        }

        protected abstract Task<IEnumerable<Mapping<TValue>>> LoadMappingsAsync(string key, IEnumerable<TValue> values);

        protected virtual Task ProcessDuplicatesAsync(Mapping<TValue> key, IEnumerable<Mapping<TValue>> duplicates) => Task.CompletedTask;

        protected void AddMapping(Mapping<TValue> mapping)
        {
            this.AddMappings(new[] { mapping });
        }

        private List<Mapping<TValue>> GetMappings(string key, IEnumerable<TValue> values)
        {
            sync.EnterReadLock();
            try
            {
                return this.knownMappings.Where(x => (x.Key == key || x.Key == null) && values.Any(y => x.Value.Equals(y))).ToList();
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        private void AddMappings(IEnumerable<Mapping<TValue>> mappings)
        {
            sync.EnterWriteLock();
            try
            {
                this.knownMappings.UnionWith(mappings);
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        public void Dispose()
        {
            sync.Dispose();
        }
    }
}
