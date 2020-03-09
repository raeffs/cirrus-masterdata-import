using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Scryfall
{
    class CollectionDto<T>
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("next_page")]
        public string NextPageUrl { get; set; }

        [JsonIgnore]
        public int NextPage => int.Parse(Regex.Match(this.NextPageUrl, "page=(?<page>\\d+)").Groups["page"].Value);

        public List<T> Data { get; set; }
    }
}
