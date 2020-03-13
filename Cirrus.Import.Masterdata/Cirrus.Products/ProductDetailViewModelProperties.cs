using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Cirrus.Products
{
    class ProductDetailViewModelProperties
    {
        public string Id { get; set; } = "0";

        public string Name { get; set; }

        public string Number { get; set; }

        public string ExternalId { get; set; }

        public string Picture { get; set; }

        public decimal? Price { get; set; }

        public List<Reference> Tax { get; set; } = new List<Reference>();

        public List<Reference> Unit { get; set; } = new List<Reference>();

        public List<Reference> ProductGroup { get; set; } = new List<Reference>();

        public List<Reference> DatacenterCode { get; set; } = new List<Reference>();

        public List<Reference> BusinessUnit { get; set; } = new List<Reference>();

        public List<Reference> TakeAwayTax { get; set; } = new List<Reference>();

        public List<Reference> RelatedDeposit { get; set; } = new List<Reference>();

        public bool Weighable { get; set; }

        public bool FreePrice { get; set; }

        public bool DiscountLock { get; set; }

        public bool PromotionLock { get; set; }

        public bool SalesLock { get; set; }

        public string DefaultQuantity { get; set; }

        public List<Reference> MinAges { get; set; } = new List<Reference>();

        public List<Reference> AllergyTypes { get; set; } = new List<Reference>();

        public string Ingredients { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public bool ForceReceiptPrint { get; set; }

        public bool CommissionFree { get; set; }

        public bool NoEffectOnMargins { get; set; }

        public string CurrencyName { get; set; }

        public string BasePriceReferenceSize { get; set; }

        public List<Reference> BasePriceUnit { get; set; } = new List<Reference>();

        public string FillingQuantity { get; set; }

        public bool PriceOnProduct { get; set; }

        public bool Editable { get; set; }

        // [JsonProperty("CoreTranslationDetail<ProductTranslation>___TranslationViewModelName")]
        // public string TranslationViewModelName { get; set; } = "CirrusModuleMasterdataDALEntitiesProductTranslations";

        // public string FuelProductExtension___ProductCode { get; set; }

        // public string FuelProductExtension___FuelingTimeoutAttended { get; set; }

        // public string FuelProductExtension___FuelingTimeoutUnattended { get; set; }

        // public string FuelProductExtension___Stock { get; set; }

        // public string HospitalityDishDetail___Description { get; set; }

        // public string HospitalityDishDetail___PreparationProcess { get; set; }

        // public string HospitalityDishDetail___PreparationTime { get; set; }

        // public List<Reference> HospitalityDishDetail___KitchenPrinterLayout { get; set; } = new List<Reference>();

        public List<Reference> LabelPrintingProductExtension___AutoPrintLabelLayout => Reference.ListFrom("-1");

        public bool PosProductExtension___IsQuantityEditable { get; set; }
    }
}
