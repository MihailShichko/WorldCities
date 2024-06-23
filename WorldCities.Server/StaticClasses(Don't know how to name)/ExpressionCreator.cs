using System.Linq.Expressions;

namespace WorldCities.Server.StaticClasses_Don_t_know_how_to_name_
{
    public static class ExpressionCreator<T>
    {
        public static Expression<Func<T, bool>> CreateFilterExpression<T>(string? filterColumn, string? filterQuery)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, filterColumn);
            var constant = Expression.Constant(filterQuery);
            var startsWith = Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);

            return Expression.Lambda<Func<T, bool>>(startsWith, parameter);
        }

        public static Expression<Func<T, object>> CreateSortExpression<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var convert = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<T, object>>(convert, parameter);
        }

    }
}
