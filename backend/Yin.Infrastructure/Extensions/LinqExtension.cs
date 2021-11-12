using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Yin.Infrastructure.Extensions
{
    public static class LinqExtension
    {
        public static IQueryable<T> WhereWhen<T>(this IQueryable<T> ts, bool condition, Expression<Func<T, bool>> expression) where T : class
        {
            return condition ? ts.Where(expression) : ts;
        }
        public static IEnumerable<T> WhereWhen<T>(this IEnumerable<T> ts, bool condition, Expression<Func<T, bool>> expression) where T : class
        {
            return condition ? ts.Where(expression.Compile()) : ts;
        }
    }
}
