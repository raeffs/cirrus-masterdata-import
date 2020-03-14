using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cirrus.Import.Masterdata.External
{
    interface ILanguageProvider : IProvider
    {
        Task<List<Language>> GetLanguagesAsync();
    }
}
