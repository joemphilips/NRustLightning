using System.Collections.Generic;
using System.Linq;

namespace NRustLightning.Tests.Common.Utils
{
    public static class Extensions
    {
        public static IEnumerable<(T, T)> Combinations2<T>(this IList<T> elements)
        {
            return elements.Combinations(2).Select(r => (r.ElementAt(0), r.ElementAt(1)));
        }

        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
        {
            return k == 0 ? new[] {new T[0]} : elements.SelectMany((e, i) =>
                {
                    return elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] {e}).Concat(c));
                });
        }
    }
}