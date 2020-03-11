using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Giantbomb
{
    class PlatformDto
    {
        public long Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        [JsonProperty("image")]
        public PictureDto Picture { get; set; }

        [JsonProperty("original_price")]
        public string Price { get; set; }
    }
}
