using System.Collections.Generic;

namespace Cirrus.Import.Masterdata.Cirrus.Products
{
    class ProductBarcode
    {
        public string Id { get; set; } = "-1";

        public string Barcode { get; set; }

        public List<Reference> ValidatorName { get; set; } = Reference.ListFrom("GTIN-13");

        public static List<ProductBarcode> ListFrom(string code)
        {
            if (code == null)
            {
                return new List<ProductBarcode>();
            }

            return new List<ProductBarcode>
            {
                new ProductBarcode
                {
                    Barcode = code
                }
            };
        }
    }
}
