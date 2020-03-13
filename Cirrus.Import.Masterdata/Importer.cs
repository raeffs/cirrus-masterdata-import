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
        private readonly AssortmentApi assortmentApi;
        private readonly CategoryApi categoryApi;
        private readonly ProductApi productApi;
        private readonly IEnumerable<ExternalProvider> providers;

        public Importer(
            AssortmentApi assortmentApi,
            CategoryApi categoryApi,
            ProductApi productApi,
            IEnumerable<ExternalProvider> providers)
        {
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
            Console.Write($"Processing assortments of {provider.Key}");
            var assortments = await provider.GetAssortmentsAsync();
            await this.assortmentApi.AddOrUpdateAsync(assortments);
            Console.WriteLine();
        }

        private async Task ProcessCategories(ExternalProvider provider)
        {
            Console.Write($"Processing categories of {provider.Key}");
            var categories = await provider.GetCategoriesAsync();
            await this.categoryApi.AddOrUpdateAsync(categories);
            Console.WriteLine();
        }

        private async Task ProcessProducts(ExternalProvider provider)
        {
            Console.WriteLine($"Processing products of {provider.Key}");
            await foreach (var products in provider.GetProductsAsync())
            {
                Console.Write($"Processing batch of {products.Count} products");
                await this.productApi.AddOrUpdateAsync(products);
                Console.WriteLine();
            }
        }
    }
}
