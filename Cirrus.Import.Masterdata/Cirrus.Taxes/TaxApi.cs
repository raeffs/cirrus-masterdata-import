using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.Cirrus.Taxes
{
    class TaxApi : MappedApi<Tax>
    {
        private readonly TaxOptions taxOptions;

        public TaxApi(TaxOptions taxOptions)
        {
            this.taxOptions = taxOptions;
        }

        protected override Task<IEnumerable<Mapping<Tax>>> LoadMappingsAsync(string key, IEnumerable<Tax> values)
        {
            return Task.FromResult(new List<Mapping<Tax>>
            {
                new Mapping<Tax> { Value = Tax.None, Id = this.taxOptions.NoneTaxId.ToString() },
                new Mapping<Tax> { Value = Tax.Default, Id = this.taxOptions.DefaultTaxId.ToString() },
                new Mapping<Tax> { Value = Tax.Reduced, Id = this.taxOptions.ReducedTaxId.ToString() },
                new Mapping<Tax> { Value = Tax.TakeAway, Id = this.taxOptions.TakeAwayTaxId.ToString() },
                new Mapping<Tax> { Value = Tax.Fuel, Id = this.taxOptions.FuelTaxId.ToString() },
            }.AsEnumerable());
        }
    }
}
