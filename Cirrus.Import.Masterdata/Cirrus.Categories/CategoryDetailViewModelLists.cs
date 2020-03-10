using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.Cirrus.Categories
{
    class CategoryDetailViewModelLists
    {

        [JsonProperty("CoreTranslationDetail<ProductCategoryTranslation>___Translations")]
        public List<object> Translations { get; set; } = new List<object>();
    }
}
