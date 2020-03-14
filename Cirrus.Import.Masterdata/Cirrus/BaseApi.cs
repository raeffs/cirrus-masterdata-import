using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cirrus.Import.Masterdata.Cirrus
{
    abstract class BaseApi<T> : IDisposable
    {
        private readonly HashSet<Mapping<T>> knownMappings = new HashSet<Mapping<T>>();
        private readonly ReaderWriterLockSlim sync = new ReaderWriterLockSlim();

        public async Task<string> GetMappingAsync(string key, T value)
        {
            return (await this.GetMappingsAsync(key, new[] { value })).Select(x => x.Id).SingleOrDefault();
        }

        public async Task<IEnumerable<Mapping<T>>> GetMappingsAsync(string key, IEnumerable<T> values)
        {
            values = values.Distinct().ToList();

            var mappings = this.GetMappings(key, values);
            var missing = values.Where(x => !mappings.Any(y => x.Equals(y.Value)));

            if (!missing.Any())
            {
                return mappings;
            }

            var loaded = await this.LoadMappingsAsync(key, missing);
            this.AddMappings(loaded);

            return this.GetMappings(key, values);
        }

        protected abstract Task<IEnumerable<Mapping<T>>> LoadMappingsAsync(string key, IEnumerable<T> values);

        protected void AddMapping(Mapping<T> mapping)
        {
            this.AddMappings(new[] { mapping });
        }

        private List<Mapping<T>> GetMappings(string key, IEnumerable<T> values)
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

        private void AddMappings(IEnumerable<Mapping<T>> mappings)
        {
            sync.EnterWriteLock();
            try
            {
                this.knownMappings.AddRange(mappings);
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
