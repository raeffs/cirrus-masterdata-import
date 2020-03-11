using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Giantbomb
{

    class PictureDto
    {
        [JsonProperty("medium_url")]
        public string Picture { get; set; }

        public override string ToString()
        {
            return this;
        }

        public static implicit operator string(PictureDto value) => value?.Picture;
    }
}
