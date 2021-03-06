﻿using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.External.TheCocktailDb
{
    class CocktailSummaryDto
    {
        [JsonProperty("idDrink")]
        public long Id { get; set; }

        [JsonProperty("strDrink")]
        public string Name { get; set; }

        [JsonProperty("strDrinkThumb")]
        public string Picture { get; set; }
    }
}
