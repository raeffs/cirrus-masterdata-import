using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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

        public static bool ContainsCategory(this JObject model, string rootCategoryId, string categoryId)
        {
            if (model == null)
            {
                return false;
            }

            var properties = model.Value<JObject>("Properties");
            if (properties == null)
            {
                return false;
            }

            var refs = properties.Value<JArray>($"Category_Id_{rootCategoryId}");
            if (refs == null)
            {
                return false;
            }

            return refs.ToObject<List<Reference>>().Contains(Reference.From(categoryId));
        }

        public static void SetCategory(this JObject model, string rootCategoryId, string categoryId)
        {
            model.Value<JObject>("Properties").Add($"Category_Id_{rootCategoryId}", JToken.FromObject(Reference.ListFrom(categoryId)));
        }
    }
}
