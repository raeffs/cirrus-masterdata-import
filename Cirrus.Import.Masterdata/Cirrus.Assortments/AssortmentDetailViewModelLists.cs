using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.Cirrus.Assortments
{
    class AssortmentDetailViewModelLists
    {
        public List<object> Sites { get; set; } = new List<object>();

        [JsonProperty("CoreTranslationDetail<ProductAssortmentTranslation>___Translations")]
        public List<object> Translations { get; set; } = new List<object>();
    }
}
