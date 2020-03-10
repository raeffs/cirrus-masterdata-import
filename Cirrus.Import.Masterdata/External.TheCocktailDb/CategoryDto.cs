using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheCocktailDb
{
    class CategoryDto
    {
        [JsonProperty("strCategory")]
        public string Name { get; set; }
    }
}
