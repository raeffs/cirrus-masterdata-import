using System;
using System.Collections.Generic;
using System.Linq;
using Cirrus.Import.Masterdata.Common;

namespace Cirrus.Import.Masterdata.Cirrus
{
    static class Extensions
    {
        public static bool ContainsReference(this IEnumerable<Reference> references, string id)
        {
            var reference = Reference.From(id);
            if (reference == null)
            {
                return true;
            }

            if (references == null)
            {
                return false;
            }

            return references.Contains(reference);
        }

        public static IEnumerable<string> Find(this IEnumerable<Mapping> mappings, BaseModel model)
        {
            return mappings.Where(x => x.Key == model.ExternalKey && x.Value == model.ExternalId).Select(x => x.Id);
        }

        public static IEnumerable<string> Find<T>(this IEnumerable<Mapping> mappings, T model, Func<T, string> idSelector)
            where T : BaseModel
        {
            return mappings.Where(x => x.Key == model.ExternalKey && x.Value == idSelector(model)).Select(x => x.Id);
        }
    }
}
