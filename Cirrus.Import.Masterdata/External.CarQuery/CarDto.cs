using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.CarQuery
{
    class CarDto
    {
        [JsonProperty("model_id")]
        public string Id { get; set; }

        [JsonProperty("model_name")]
        public string Name { get; set; }

        [JsonProperty("model_trim")]
        public string Trim { get; set; }

        [JsonProperty("model_year")]
        public string Year { get; set; }

        [JsonProperty("model_body")]
        public string Body { get; set; }

        public string FullName
        {
            get
            {
                var name = this.Make;
                if (!string.IsNullOrWhiteSpace(this.Name))
                {
                    name += $" {this.Name}";
                }
                if (!string.IsNullOrWhiteSpace(this.Trim))
                {
                    name += $" {this.Trim}";
                }
                if (!string.IsNullOrWhiteSpace(this.Body))
                {
                    name += $", {this.Body}";
                }
                if (!string.IsNullOrWhiteSpace(this.Year))
                {
                    name += $" ({this.Year})";
                }
                return name;
            }
        }

        [JsonProperty("make_display")]
        public string Make { get; set; }
    }

    class CarCollectionDto
    {
        public List<CarDto> Trims { get; set; }
    }
}
