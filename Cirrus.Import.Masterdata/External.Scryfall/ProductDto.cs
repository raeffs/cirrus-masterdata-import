using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Scryfall
{
    class ProductDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("image_uris")]
        public ProductImageDto Picture { get; set; }

        [JsonProperty("prices")]
        public ProductPriceDto Price { get; set; }

        public Rarity Rarity { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }
    }

    class ProductImageDto
    {
        public string Png { get; set; }

        public override string ToString()
        {
            return this;
        }

        public static implicit operator string(ProductImageDto value) => value?.Png;
    }

    class ProductPriceDto
    {
        public string Usd { get; set; }

        public string Eur { get; set; }

        public override string ToString()
        {
            return this;
        }

        public static implicit operator string(ProductPriceDto value) =>
            !string.IsNullOrWhiteSpace(value?.Usd) ? value.Usd :
            !string.IsNullOrWhiteSpace(value?.Eur) ? value.Eur :
            null;
    }

    enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Mythic
    }
}
