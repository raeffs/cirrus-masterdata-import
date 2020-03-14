using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.Swapi
{
    class SwapiProvider : ExternalProvider
    {
        private readonly string RootCategoryId = "Vehicles & Starships";
        private readonly string StarshipAssortmentId = "Starships";
        private readonly string StarshipCategoryId = "Starships";
        private readonly string VehicleAssortmentId = "Vehicles";
        private readonly string VehicleCategoryId = "Vehicles";
        private readonly SwapiOptions options;

        public bool Enabled => this.options.Enabled;

        public string Key => "swapi";

        public SwapiProvider(SwapiOptions options)
        {
            this.options = options;
        }

        public Task<List<Assortment>> GetAssortmentsAsync()
        {
            return Task.FromResult(new List<Assortment>
            {
                new Assortment
                {
                    ExternalKey = this.Key,
                    ExternalId = this.VehicleAssortmentId
                },
                new Assortment
                {
                    ExternalKey = this.Key,
                    ExternalId = this.StarshipAssortmentId
                }
            });
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            return Task.FromResult(new List<Category>
            {
                new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = this.RootCategoryId
                },
                new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = this.VehicleCategoryId,
                    ExternalParentId = this.RootCategoryId
                },
                new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = this.StarshipCategoryId,
                    ExternalParentId = this.RootCategoryId
                }
            });
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            CollectionDto<VehicleAndStarshipDto> result = null;

            do
            {
                result = await this.GetClient()
                    .AppendPathSegment("vehicles")
                    .SetQueryParam("page", result?.NextPage ?? 1)
                    .GetJsonAsync<CollectionDto<VehicleAndStarshipDto>>();

                yield return result.Results
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id.ToString(),
                        Name = x.Name,
                        ExternalAssortmentId = this.VehicleAssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.From(x.Price, 10) ?? Price.FromId(x.Id, 100000),
                        ExternalCategoryIds = new List<string> { this.VehicleCategoryId }
                    })
                    .ToList();
            }
            while (result.HasMore);

            result = null;

            do
            {
                result = await this.GetClient()
                    .AppendPathSegment("starships")
                    .SetQueryParam("page", result?.NextPage ?? 1)
                    .GetJsonAsync<CollectionDto<VehicleAndStarshipDto>>();

                yield return result.Results
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id.ToString(),
                        Name = x.Name,
                        ExternalAssortmentId = this.StarshipAssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.From(x.Price, 10) ?? Price.FromId(x.Id, 1000000),
                        ExternalCategoryIds = new List<string> { this.StarshipCategoryId }
                    })
                    .ToList();
            }
            while (result.HasMore);
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://swapi.co/api");
        }
    }
}
