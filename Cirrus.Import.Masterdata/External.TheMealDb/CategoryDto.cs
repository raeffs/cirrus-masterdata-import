using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheMealDb
{
    class CategoryDto
    {
        [JsonProperty("strCategory")]
        public string Name { get; set; }
    }
}
