using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Husky
{
	public static class QueryableHelper
	{
		#region OrderBy

		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyPath, SortDirection sortDirection = SortDirection.Ascending) {
			var propertyType = GetPropertyType<T>(propertyPath);
			var methodName = sortDirection == SortDirection.Descending ? nameof(_OrderByDescending) : nameof(_OrderBy);
			var method = typeof(QueryableHelper).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(typeof(T), propertyType);
			return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, propertyPath })!;
		}
		public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyPath) => OrderBy(query, propertyPath, SortDirection.Descending);

		#endregion

		#region ThenBy

		public static IOrderedQueryable<T> ThenBy<T>(this IQueryable<T> query, string propertyPath, SortDirection sortDirection = SortDirection.Ascending) {
			var propertyType = GetPropertyType<T>(propertyPath);
			var methodName = sortDirection == SortDirection.Descending ? nameof(_ThenByDescending) : nameof(_ThenBy);
			var method = typeof(QueryableHelper).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(typeof(T), propertyType);
			return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, propertyPath })!;
		}
		public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyPath) => ThenBy(query, propertyPath, SortDirection.Descending);

		#endregion

		#region Where

		public static IQueryable<T> Where<T>(this IQueryable<T> query, string propertyPath, object filterValue, Comparison comparison = Comparison.Equal) => query.Where(Predicate<T>(propertyPath, filterValue, comparison));

		#endregion

		#region Select

		public static Expression<Func<T, TReturn>> SelectProperty<T, TReturn>(string propertyName) => Selector<T, TReturn>(propertyName);

		#endregion

		#region Supports

#pragma warning disable IDE1006 // Naming Styles
		private static IOrderedQueryable<T> _OrderBy<T, TProp>(IQueryable<T> query, string propertyPath) => query.OrderBy(Selector<T, TProp>(propertyPath));
		private static IOrderedQueryable<T> _OrderByDescending<T, TProp>(IQueryable<T> query, string propertyPath) => query.OrderByDescending(Selector<T, TProp>(propertyPath));
		private static IOrderedQueryable<T> _ThenBy<T, TProp>(IOrderedQueryable<T> query, string propertyPath) => query.ThenBy(Selector<T, TProp>(propertyPath));
		private static IOrderedQueryable<T> _ThenByDescending<T, TProp>(IOrderedQueryable<T> query, string propertyPath) => query.ThenByDescending(Selector<T, TProp>(propertyPath));
#pragma warning restore IDE1006 // Naming Styles

		private static Type GetPropertyType<T>(string propertyPath) {
			var type = typeof(T);
			var steps = propertyPath.Split('.');
			foreach ( var propertyName in steps ) {
				type = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance)!.PropertyType;
			}
			return type;
		}

		private static Expression GetPropertyExpression<T>(ParameterExpression arg, string propertyPath) {
			Expression? property = null;
			var steps = propertyPath.Split('.');
			foreach ( var propertyName in steps ) {
				property = Expression.Property(property ?? arg, propertyName);
			}
			return property!;
		}

		private static Expression<Func<T, TReturn>> Selector<T, TReturn>(string propertyPath) {
			var arg = Expression.Parameter(typeof(T));
			var property = GetPropertyExpression<T>(arg, propertyPath);
			return Expression.Lambda<Func<T, TReturn>>(property, arg);
		}

		private static Expression<Func<T, bool>> Predicate<T>(string propertyPath, object value, Comparison comparison) {
			var arg = Expression.Parameter(typeof(T));
			var property = GetPropertyExpression<T>(arg, propertyPath);

			//Get type of value
			var typeofValue = property.Type;
			//when type of value is Nullable<T>, then T is what we want
			if ( typeofValue.IsGenericType ) {
				typeofValue = typeofValue.GenericTypeArguments[0];
			}

			//hack: kendo datetime format.
			//todo: polish
			if ( typeofValue == typeof(DateTime) ) {
				var datestr = value as string ?? value.ToString();
				if ( datestr != null && datestr.Contains('(') && datestr.Contains(')') && datestr.Contains("GMT") ) {
					value = datestr.Substring(4, 11);
				}
			}

			//Get value expression
			var exprValue = typeofValue == value.GetType()
				? Expression.Constant(value, property.Type)

					//: typeofValue == typeof(string)
					//	? Expression.Constant(value.ToString(), property.Type)

					: (typeofValue.IsEnum || typeof(IConvertible).IsAssignableFrom(typeofValue))
						? Expression.Constant(TypeDescriptor.GetConverter(typeofValue).ConvertFrom(value), property.Type)

						: (typeofValue == typeof(Guid) && value is string str)
							? Expression.Constant(Guid.Parse(str), typeof(Guid))

							: Expression.Constant(Convert.ChangeType(value, typeofValue), property.Type)
							;

			//Get lambda expression
			Expression lambda = comparison switch
			{
				Comparison.Equal => Expression.Equal(property, exprValue),
				Comparison.NotEqual => Expression.NotEqual(property, exprValue),
				Comparison.GreaterThan => Expression.GreaterThan(property, exprValue),
				Comparison.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, exprValue),
				Comparison.LessThan => Expression.LessThan(property, exprValue),
				Comparison.LessThanOrEqual => Expression.LessThanOrEqual(property, exprValue),
				Comparison.HasKeyword => Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!, exprValue),
				_ => throw new ArgumentOutOfRangeException(nameof(comparison)),
			};

			return Expression.Lambda<Func<T, bool>>(lambda, arg);
		}

		#endregion
	}
}