using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Giantbomb
{
    class CollectionDto<T>
    {
        public List<T> Results { get; set; }

        [JsonProperty("limit")]
        public int PageSize { get; set; }

        public int Offset { get; set; }

        [JsonProperty("number_of_total_results")]
        public int TotalCount { get; set; }

        [JsonIgnore]
        public bool HasMore => this.PageSize + this.Offset < this.TotalCount;

        [JsonIgnore]
        public int NextOffset => this.PageSize + this.Offset;
    }
}
