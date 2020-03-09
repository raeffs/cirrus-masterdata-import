using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheMealDb
{
    class CollectionDto<T>
    {
        [JsonProperty("meals")]
        public List<T> Items { get; set; }
    }
}
