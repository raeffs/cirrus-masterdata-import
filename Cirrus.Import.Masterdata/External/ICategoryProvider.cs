using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.External
{
    interface ICategoryProvider : IProvider, IAssortmentProvider
    {
        Task<List<Category>> GetCategoriesAsync();
    }
}
