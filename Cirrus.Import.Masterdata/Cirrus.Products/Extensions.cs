using System.Collections.Generic;
using System.Linq;

namespace Cirrus.Import.Masterdata.Cirrus.Products
{
    static class Extensions
    {
        public static bool ContainsCode(this IEnumerable<ProductBarcode> barcodes, string code)
        {
            if (code == null)
            {
                return true;
            }

            return barcodes.Any(x => x.Barcode == code);
        }
    }
}
