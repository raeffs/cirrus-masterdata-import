using System.Collections.Generic;
using System.Linq;

namespace Cirrus.Import.Masterdata
{
    static class Extensions
    {
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                set.Add(value);
            }
        }

        public static bool ContainsSameElements(this IEnumerable<string> a, IEnumerable<string> b)
        {
            a = a ?? new List<string>();
            b = b ?? new List<string>();
            return !a.Except(b).Union(b.Except(a)).Any();
        }
    }
}
