using System.Collections.Generic;

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
    }
}
