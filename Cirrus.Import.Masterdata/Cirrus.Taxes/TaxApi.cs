using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.Cirrus.Taxes
{
    class TaxApi
    {
        private readonly TaxOptions taxOptions;

        public TaxApi(TaxOptions taxOptions)
        {
            this.taxOptions = taxOptions;
        }

        public Task<List<CustomMapping<Tax>>> GetMappingsAsync()
        {
            return Task.FromResult(new List<CustomMapping<Tax>>
            {
                new CustomMapping<Tax> { Value = Tax.None, Id = this.taxOptions.NoneTaxId.ToString() },
                new CustomMapping<Tax> { Value = Tax.Default, Id = this.taxOptions.DefaultTaxId.ToString() },
                new CustomMapping<Tax> { Value = Tax.Reduced, Id = this.taxOptions.ReducedTaxId.ToString() },
                new CustomMapping<Tax> { Value = Tax.TakeAway, Id = this.taxOptions.TakeAwayTaxId.ToString() },
                new CustomMapping<Tax> { Value = Tax.Fuel, Id = this.taxOptions.FuelTaxId.ToString() },
            });
        }
    }
}
