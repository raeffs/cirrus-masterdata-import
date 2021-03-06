﻿using System.Collections.Generic;
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

        public static bool ContainsAllCategories(this JObject model, string rootCategoryId, IEnumerable<string> categoryIds)
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

            var actual = refs.ToObject<List<Reference>>();
            var expected = Reference.ListFrom(categoryIds);

            var result = !actual.Except(expected).Union(expected.Except(actual)).Any();
            return result;
        }

        public static void SetCategories(this JObject model, string rootCategoryId, IEnumerable<string> categoryIds)
        {
            model.Value<JObject>("Properties").Add($"Category_Id_{rootCategoryId}", JToken.FromObject(Reference.ListFrom(categoryIds)));
        }
    }
}
