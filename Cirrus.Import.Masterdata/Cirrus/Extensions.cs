using System;
using System.Collections.Generic;
using System.Linq;

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

            return references.Contains(reference);
        }
    }
}
