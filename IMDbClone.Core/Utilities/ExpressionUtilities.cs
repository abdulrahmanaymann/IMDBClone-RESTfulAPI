using System.Linq.Expressions;

namespace IMDbClone.Core.Utilities
{
    public static class ExpressionUtilities
    {
        /// <summary>
        /// Combines two lambda expressions of the same type using a logical AND operation.
        /// </summary>
        /// <typeparam name="T">The type of the parameter in the lambda expressions.</typeparam>
        /// <param name="first">The first expression to combine.</param>
        /// <param name="second">The second expression to combine.</param>
        /// <returns>A new expression that represents the logical AND of the two input expressions.</returns>
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