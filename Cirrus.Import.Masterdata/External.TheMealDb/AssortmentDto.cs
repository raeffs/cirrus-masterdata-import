using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheMealDb
{
    class AssortmentDto
    {
        [JsonProperty("strCategory")]
        public string Name { get; set; }
    }
}
