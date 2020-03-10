using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cirrus.Import.Masterdata.Common
{
    interface ExternalProvider
    {
        string Key { get; }

        Task<List<Assortment>> GetAssortmentsAsync();

        Task<List<Category>> GetCategoriesAsync();

        IAsyncEnumerable<List<Product>> GetProductsAsync();
    }
}
