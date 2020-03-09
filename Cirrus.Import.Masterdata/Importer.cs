using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Cirrus.Assortments;
using Cirrus.Import.Masterdata.Cirrus.Groups;
using Cirrus.Import.Masterdata.Cirrus.Products;
using Cirrus.Import.Masterdata.Cirrus.Taxes;
using Cirrus.Import.Masterdata.Cirrus.Units;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata
{
    class Importer
    {
        private readonly UnitApi unitApi;
        private readonly TaxApi taxApi;
        private readonly GroupApi groupApi;
        private readonly AssortmentApi assortmentApi;
        private readonly ProductApi productApi;
        private readonly IEnumerable<ExternalProvider> providers;

        public Importer(
            UnitApi unitApi,
            TaxApi taxApi,
            GroupApi groupApi,
            AssortmentApi assortmentApi,
            ProductApi productApi,
            IEnumerable<ExternalProvider> providers)
        {
            this.unitApi = unitApi;
            this.taxApi = taxApi;
            this.groupApi = groupApi;
            this.assortmentApi = assortmentApi;
            this.productApi = productApi;
            this.providers = providers;
        }

        public async Task Import()
        {
            foreach (var provider in this.providers)
            {
                await this.ProcessAssortments(provider);
                await this.ProcessProducts(provider);
            }
        }

        private async Task ProcessAssortments(ExternalProvider provider)
        {
            Console.Write($"Processing assortments of {provider.Key}");
            var assortments = await provider.GetAssortmentsAsync();
            var assortmentMappings = await this.assortmentApi.GetMappingsAsync(provider.Key, assortments.Select(x => x.ExternalId).ToList());
            foreach (var assortment in assortments)
            {
                Console.Write($"\rProcessing assortments of {provider.Key}: {assortments.IndexOf(assortment) + 1} / {assortments.Count}");
                assortment.Id = assortmentMappings.Where(x => x.Value == assortment.ExternalId).Select(x => x.Id).SingleOrDefault();
                await this.assortmentApi.AddOrUpdateAsync(assortment);
            }
            Console.WriteLine();
        }

        private async Task ProcessProducts(ExternalProvider provider)
        {
            Console.WriteLine($"Processing products of {provider.Key}");
            await foreach (var products in provider.GetProductsAsync())
            {
                Console.Write($"Processing batch of {products.Count} products");
                var productMappings = await this.productApi.GetMappingsAsync(provider.Key, products.Select(x => x.ExternalId).ToList());
                var assortmentMappings = await this.assortmentApi.GetMappingsAsync(provider.Key, products.Select(x => x.ExternalAssortmentId).ToList());
                var unitMapping = await this.unitApi.GetMappingsAsync();
                var taxMapping = await this.taxApi.GetMappingsAsync();
                var groupMapping = await this.groupApi.GetMappingsAsync();
                foreach (var product in products)
                {
                    Console.Write($"\rProcessing batch of {products.Count} products: {products.IndexOf(product) + 1} / {products.Count}");
                    product.Id = productMappings.Where(x => x.Value == product.ExternalId).Select(x => x.Id).SingleOrDefault();
                    product.AssortmentId = assortmentMappings.Where(x => x.Value == product.ExternalAssortmentId).Select(x => x.Id).Single();
                    product.UnitId = unitMapping.Where(x => x.Value == product.ExternalUnit).Select(x => x.Id).Single();
                    product.TaxId = taxMapping.Where(x => x.Value == product.ExternalTax).Select(x => x.Id).Single();
                    product.GroupId = groupMapping.Where(x => x.Value == product.ExternalGroup).Select(x => x.Id).Single();
                    await this.productApi.AddOrUpdateAsync(product);
                }
                Console.WriteLine();
            }
        }
    }
}
