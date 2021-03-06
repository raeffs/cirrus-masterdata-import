﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;
using Flurl.Http;

namespace Cirrus.Import.Masterdata.External.TheMealDb
{
    class TheMealDbProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Meals";
        private readonly string RootCategoryId = "Meals";
        private readonly TheMealDbOptions options;
        private IReadOnlyList<Category> categories;

        public bool Enabled => this.options.Enabled;

        public string Key => "the-meal-db";

        public TheMealDbProvider(TheMealDbOptions options)
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

        public async Task<List<Category>> GetCategoriesAsync()
        {
            if (this.categories != null)
            {
                return this.categories.ToList();
            }

            var categories = new List<Category>
            {
                new Category { ExternalKey = this.Key, ExternalId = this.RootCategoryId }
            };

            var result = await this.GetClient()
                .AppendPathSegment("list.php")
                .SetQueryParam("c", "list")
                .GetJsonAsync<CollectionDto<CategoryDto>>();

            categories.AddRange(result.Items
                .Select(x => new Category
                {
                    ExternalKey = this.Key,
                    ExternalId = x.Name,
                    ExternalParentId = this.RootCategoryId
                })
                .ToList());

            this.categories = categories;
            return this.categories.ToList();
        }

        public async IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            foreach (var category in await this.GetCategoriesAsync())
            {
                if (!category.IsChild)
                {
                    continue;
                }

                var result = await this.GetClient()
                    .AppendPathSegment("filter.php")
                    .SetQueryParam("c", category.ExternalId)
                    .GetJsonAsync<CollectionDto<MealSummaryDto>>();

                yield return result.Items
                    .Select(x => new Product
                    {
                        ExternalKey = this.Key,
                        ExternalId = x.Id.ToString(),
                        Name = x.Name,
                        ExternalAssortmentId = this.AssortmentId,
                        ExternalUnit = Unit.Piece,
                        ExternalTax = Tax.Default,
                        ExternalGroup = Group.Default,
                        Barcode = Barcode.FromId(this.Key, x.Id),
                        Price = Price.FromId(x.Id, 30),
                        Picture = x.Picture,
                        ExternalCategoryIds = new List<string> { category.ExternalId }
                    })
                    .ToList();
            }
        }

        private IFlurlRequest GetClient()
        {
            return new FlurlRequest("https://www.themealdb.com/api/json/v1/1");
        }
    }
}
