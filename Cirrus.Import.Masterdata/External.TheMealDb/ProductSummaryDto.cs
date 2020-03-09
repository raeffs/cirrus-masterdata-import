using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheMealDb
{
    class ProductSummaryDto
    {
        [JsonProperty("idMeal")]
        public long Id { get; set; }

        [JsonProperty("strMeal")]
        public string Name { get; set; }

        [JsonProperty("strMealThumb")]
        public string Picture { get; set; }
    }
}
