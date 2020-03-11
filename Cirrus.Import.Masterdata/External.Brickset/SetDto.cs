using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.Brickset
{
    class SetDto
    {
        public string SetId { get; set; }

        public string Number { get; set; }

        public string Name { get; set; }

        [JsonProperty("ImageUrl")]
        public string Picture { get; set; }

        [JsonIgnore]
        public string Price =>
            EuRetailPrice ??
            UsRetailPrice ??
            UkRetailPrice;

        public string UsRetailPrice { get; set; }

        public string UkRetailPrice { get; set; }

        public string EuRetailPrice { get; set; }

        public string PackagingType { get; set; }
    }

    class ArrayOfSetsDto
    {
        public List<SetDto> Sets { get; set; }
    }

    class SetCollectionDto
    {
        public ArrayOfSetsDto ArrayOfSets { get; set; }
    }
}
