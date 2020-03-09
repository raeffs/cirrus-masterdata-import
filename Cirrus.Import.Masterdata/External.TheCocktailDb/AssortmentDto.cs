using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheCocktailDb
{
    class AssortmentDto
    {
        [JsonProperty("strCategory")]
        public string Name { get; set; }
    }
}
