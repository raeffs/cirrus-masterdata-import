﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.Scryfall
{
    class ScryfallProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Magic The Gathering Cards";
        private readonly string RootCategoryId = "Magic The Gathering Cards";
        private readonly ScryfallOptions options;

        public bool Enabled => this.options.Enabled;

        public string Key => "scryfall";

        public ScryfallProvider(ScryfallOptions options)
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
                    ExternalId = this.AssortmentId
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
                }
            });
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            CollectionDto<ProductDto> result = null;

            do
            {
                result = await this.GetClient()
                    .AppendPathSegment("cards")
                    .SetQueryParam("page", result?.NextPage ?? 1)
                    .GetJsonAsync<CollectionDto<ProductDto>>();

                yield return result.Data
                    // the API does not allow us to filter, so we do it afterwards
                    // we are only intrested in the english cards, as the other languages are just duplicates
                    .Where(x => x.Language == "en")
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id,
                        Name = x.Name,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, this.GetMaxPrice(x.Rarity)),
                        Picture = x.Picture,
                        ExternalCategoryId = this.RootCategoryId
                    })
                    .ToList();
            }
            while (result.HasMore);
        }

        private int GetMaxPrice(Rarity rarity) => rarity switch
        {
            Rarity.Common => 1,
            Rarity.Uncommon => 5,
            Rarity.Rare => 20,
            _ => 100
        };

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://api.scryfall.com/");
        }
    }
}
