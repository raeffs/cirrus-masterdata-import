using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.PokemonTcg
{
    class ProductDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("imageUrl")]
        public string Picture { get; set; }

        public string Rarity { get; set; }
    }
}
