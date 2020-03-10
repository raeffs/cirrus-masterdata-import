namespace Cirrus.Import.Masterdata.Common
{
    class Product : BaseModel
    {
        public Length64String Name { get; set; }

        public string ExternalAssortmentId { get; set; }

        public string AssortmentId { get; set; }

        public Unit ExternalUnit { get; set; }

        public string UnitId { get; set; }

        public Tax ExternalTax { get; set; }

        public string TaxId { get; set; }

        public Group ExternalGroup { get; set; }

        public string GroupId { get; set; }

        public Barcode Barcode { get; set; }

        public Price Price { get; set; }

        public string Picture { get; set; }

        public string ExternalCategoryId { get; set; }

        public string CategoryId { get; set; }

        public string RootCategoryId { get; set; }
    }
}
