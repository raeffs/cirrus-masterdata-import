using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.External.Comicvine
{
    class ComicvineProvider : ExternalProvider
    {
        private readonly string AssortmentId = "Games";
        private readonly string RootCategoryId = "Games";
        private readonly string HardwareAssortmentId = "Hardware";
        private readonly string HardwareCategoryId = "Hardware";
        private readonly ComicvineOptions options;
        private IReadOnlyList<Category> categories;
        private IReadOnlyList<Product> products;

        public bool Enabled => this.options.Enabled;

        public string Key => "giantbomb";

        public ComicvineProvider(ComicvineOptions options)
        {
            this.options = options;
        }

        public Task<List<Assortment>> GetAssortmentsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Category>> GetCategoriesAsync()
        {
            throw new System.NotImplementedException();
        }

        public IAsyncEnumerable<List<Product>> GetProductsAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
