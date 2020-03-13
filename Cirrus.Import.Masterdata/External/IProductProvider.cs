using System.Collections.Generic;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.External
{
    interface IProductProvider : IProvider, ICategoryProvider
    {
        IAsyncEnumerable<List<Product>> GetProductsAsync();
    }
}
