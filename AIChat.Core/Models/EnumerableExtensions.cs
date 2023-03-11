using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIChat.Core.Models;
internal static class EnumerableExtensions
{
    public static IEnumerable<T> TakeByLimit<T>(this IEnumerable<T> values, int limit, Func<T, int> getCost)
    {
        var count = 0;
        foreach (var item in values)
        {
            count += getCost(item);

            if(limit <= count)
            {
                yield break;
            }

            yield return item;
        }
    }

    public static IEnumerable<T> TakeByLimitDesc<T>(this IEnumerable<T> values, int limit, Func<T, int> getCost) => values.Reverse().TakeByLimit(limit, getCost).Reverse();
}
