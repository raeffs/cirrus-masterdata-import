using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Cirrus.Assortments;
using Cirrus.Import.Masterdata.Cirrus.Categories;
using Cirrus.Import.Masterdata.Cirrus.Products;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata
{
    class Importer
    {
        private readonly ApiOptions options;
        private readonly AssortmentApi assortmentApi;
        private readonly CategoryApi categoryApi;
        private readonly ProductApi productApi;
        private readonly IEnumerable<ExternalProvider> providers;

        public Importer(
            ApiOptions options,
            AssortmentApi assortmentApi,
            CategoryApi categoryApi,
            ProductApi productApi,
            IEnumerable<ExternalProvider> providers)
        {
            this.options = options;
            this.assortmentApi = assortmentApi;
            this.categoryApi = categoryApi;
            this.productApi = productApi;
            this.providers = providers;
        }

        public async Task Import()
        {
            foreach (var provider in this.providers.Where(x => x.Enabled))
            {
                await this.ProcessAssortments(provider);
                await this.ProcessCategories(provider);
                await this.ProcessProducts(provider);
            }
        }

        private async Task ProcessAssortments(ExternalProvider provider)
        {
            await Console.Out.WriteLineAsync($"Processing assortments of {provider.Key}");
            var assortments = await provider.GetAssortmentsAsync();
            await this.assortmentApi.AddOrUpdateAsync(assortments);
        }

        private async Task ProcessCategories(ExternalProvider provider)
        {
            await Console.Out.WriteLineAsync($"Processing categories of {provider.Key}");
            var categories = await provider.GetCategoriesAsync();
            await this.categoryApi.AddOrUpdateAsync(categories);
        }

        private async Task ProcessProducts(ExternalProvider provider)
        {
            await Console.Out.WriteLineAsync($"Processing products of {provider.Key}");
            await foreach (var products in provider.GetProductsAsync())
            {
                await Console.Out.WriteLineAsync($"Processing batch of {products.Count} products");
                await this.productApi.AddOrUpdateAsync(products);
            }
        }
    }
}
