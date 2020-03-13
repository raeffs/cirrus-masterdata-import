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

            if (references == null)
            {
                return false;
            }

            return references.Contains(reference);
        }

        public static bool ContainsReferences(this IEnumerable<Reference> candidates, IEnumerable<string> ids)
        {
            candidates = candidates ?? new List<Reference>();
            var references = Reference.ListFrom(ids);
            return !candidates.Except(references).Union(references.Except(candidates)).Any();

        }
    }
}
