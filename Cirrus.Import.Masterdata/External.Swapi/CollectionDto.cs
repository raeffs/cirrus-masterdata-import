using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Cirrus.Import.Masterdata.External.Swapi
{
    class CollectionDto<T>
    {
        public int Count { get; set; }

        public string Next { get; set; }

        public List<T> Results { get; set; }

        [JsonIgnore]
        public int NextPage => int.Parse(Regex.Match(this.Next, "page=(?<page>\\d+)").Groups["page"].Value);

        [JsonIgnore]
        public bool HasMore => !string.IsNullOrWhiteSpace(this.Next);
    }
}
