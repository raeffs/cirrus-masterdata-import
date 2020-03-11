using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.PunkApi
{
    class BeerDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("image_url")]
        public string Picture { get; set; }
    }
}
