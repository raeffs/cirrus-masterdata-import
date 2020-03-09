using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.PokemonTcg
{
    class CollectionDto<T>
    {
        [JsonProperty("cards")]
        public List<T> Items { get; set; }
    }
}
