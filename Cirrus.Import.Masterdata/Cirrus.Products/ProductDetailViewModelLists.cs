using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cirrus.Import.Masterdata.Cirrus.Products
{
    class ProductDetailViewModelLists
    {
        public List<Mapping> Mappings { get; set; } = new List<Mapping>();

        public List<Reference> ProductAssortments { get; set; } = new List<Reference>();

        public List<ProductBarcode> Barcodes { get; set; } = new List<ProductBarcode>();

        public List<object> Components { get; set; } = new List<object>();

        [JsonProperty("CoreTranslationDetail<ProductTranslation>___Translations")]
        public List<object> Translations { get; set; } = new List<object>();

        public List<object> HospitalityDishDetail___DishIngredients { get; set; } = new List<object>();

        public List<object> HospitalityDishDetail___DishCustomizationItems { get; set; } = new List<object>();

        public List<object> HospitalityDishDetail___DishComponents { get; set; } = new List<object>();
    }
}
