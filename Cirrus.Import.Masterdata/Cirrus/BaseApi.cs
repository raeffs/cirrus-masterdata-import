using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cirrus.Import.Masterdata.Cirrus
{
    abstract class BaseApi<T>
    {
        private readonly HashSet<Mapping<T>> knownMappings = new HashSet<Mapping<T>>();

        public async Task<string> GetMappingAsync(string key, T value)
        {
            return (await this.GetMappingsAsync(key, new[] { value })).Select(x => x.Id).SingleOrDefault();
        }

        public async Task<IEnumerable<Mapping<T>>> GetMappingsAsync(string key, IEnumerable<T> values)
        {
            values = values.Distinct().ToList();

            var mappings = this.knownMappings.Where(x => (x.Key == key || x.Key == null) && values.Any(y => x.Value.Equals(y)));
            var missing = values.Where(x => !mappings.Any(y => x.Equals(y.Value)));

            if (!missing.Any())
            {
                return mappings;
            }

            var loaded = await this.LoadMappingsAsync(key, missing);
            this.knownMappings.AddRange(loaded);

            return this.knownMappings.Where(x => (x.Key == key || x.Key == null) && values.Any(y => x.Value.Equals(y)));
        }

        protected abstract Task<IEnumerable<Mapping<T>>> LoadMappingsAsync(string key, IEnumerable<T> values);

        protected void AddMapping(Mapping<T> mapping)
        {
            this.knownMappings.Add(mapping);
        }
    }
}
