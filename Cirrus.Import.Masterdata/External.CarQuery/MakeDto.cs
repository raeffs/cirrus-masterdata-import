using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.CarQuery
{
    class MakeDto
    {
        [JsonProperty("make_display")]
        public string Name { get; set; }
    }

    class MakeCollectionDto
    {
        public List<MakeDto> Makes { get; set; }
    }
}
