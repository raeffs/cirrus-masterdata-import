using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.PokemonTcg
{
    class CardDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string Picture { get; set; }

        public string Rarity { get; set; }

        public string Set { get; set; }
    }

    class CardCollectionDto
    {
        public List<CardDto> Cards { get; set; }
    }
}
