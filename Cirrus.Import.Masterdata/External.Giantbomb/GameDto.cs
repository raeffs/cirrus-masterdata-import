using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Giantbomb
{
    class GameDto
    {
        public long Id { get; set; }

        public string Guid { get; set; }

        public string Name { get; set; }

        [JsonProperty("image")]
        public PictureDto Picture { get; set; }

        public List<PlatformDto> Platforms { get; set; }
    }
}
