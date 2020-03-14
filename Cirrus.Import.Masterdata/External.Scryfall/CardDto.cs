using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Scryfall
{
    class CardDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("image_uris")]
        public CardImageDto Picture { get; set; }

        [JsonProperty("prices")]
        public CardPriceDto Price { get; set; }

        public Rarity Rarity { get; set; }

        [JsonProperty("lang")]
        public string Language { get; set; }

        [JsonProperty("set_name")]
        public string SetName { get; set; }
    }

    class CardImageDto
    {
        public string Png { get; set; }

        public override string ToString()
        {
            return this;
        }

        public static implicit operator string(CardImageDto value) => value?.Png;
    }

    class CardPriceDto
    {
        public string Usd { get; set; }

        public string Eur { get; set; }

        public override string ToString()
        {
            return this;
        }

        public static implicit operator string(CardPriceDto value) =>
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
