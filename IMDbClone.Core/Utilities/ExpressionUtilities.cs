using System.Linq.Expressions;

namespace IMDbClone.Core.Utilities
{
    public static class ExpressionUtilities
    {
        public static Expression<Func<T, bool>> CombineExpressions<T>(
            Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var combinedBody = Expression.AndAlso(
                Expression.Invoke(first, parameter),
                Expression.Invoke(second, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(combinedBody, parameter);
        }
    }
}