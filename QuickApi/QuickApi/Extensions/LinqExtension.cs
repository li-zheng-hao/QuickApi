using System;
using System.Linq;
using System.Linq.Expressions;

namespace System.Linq
{
    public static class LinqExtension
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source,
            bool condition,
            Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
    }
}
