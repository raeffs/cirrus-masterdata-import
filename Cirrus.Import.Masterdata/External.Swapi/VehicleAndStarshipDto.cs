using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Swapi
{
    class VehicleAndStarshipDto
    {
        [JsonIgnore]
        public int Id => int.Parse(Regex.Match(this.Url, "\\/(?<id>\\d+)\\/").Groups["id"].Value);

        public string Name { get; set; }

        public string Url { get; set; }

        [JsonProperty("cost_in_credits")]
        public string Price { get; set; }
    }
}
