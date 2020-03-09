using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheCocktailDb
{
    class CollectionDto<T>
    {
        [JsonProperty("drinks")]
        public List<T> Items { get; set; }
    }
}
