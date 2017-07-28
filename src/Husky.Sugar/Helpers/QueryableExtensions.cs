using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Husky.Sugar
{
	public static class QueryableExtensions
	{
		#region OrderBy

		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyPath, SortDirection sortDirection = SortDirection.Ascending) {
			var propertyType = GetPropertyType<T>(propertyPath);
			var methodName = sortDirection == SortDirection.Descending ? nameof(_OrderByDescending) : nameof(_OrderBy);
			var method = typeof(QueryableExtensions).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeof(T), propertyType);
			return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, propertyPath });
		}
		public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> query, string propertyPath) => OrderBy(query, propertyPath, SortDirection.Descending);

		private static IOrderedQueryable<T> _OrderBy<T, TProp>(IQueryable<T> query, string propertyPath) => query.OrderBy(Selector<T, TProp>(propertyPath));
		private static IOrderedQueryable<T> _OrderByDescending<T, TProp>(IQueryable<T> query, string propertyPath) => query.OrderByDescending(Selector<T, TProp>(propertyPath));


		#endregion

		#region ThenBy

		public static IOrderedQueryable<T> ThenBy<T>(this IQueryable<T> query, string propertyPath, SortDirection sortDirection = SortDirection.Ascending) {
			var propertyType = GetPropertyType<T>(propertyPath);
			var methodName = sortDirection == SortDirection.Descending ? nameof(_ThenByDescending) : nameof(_ThenBy);
			var method = typeof(QueryableExtensions).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(typeof(T), propertyType);
			return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, propertyPath });
		}
		public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> query, string propertyPath) => OrderBy(query, propertyPath, SortDirection.Descending);

		private static IOrderedQueryable<T> _ThenBy<T, TProp>(IOrderedQueryable<T> query, string propertyPath) => query.ThenBy(Selector<T, TProp>(propertyPath));
		private static IOrderedQueryable<T> _ThenByDescending<T, TProp>(IOrderedQueryable<T> query, string propertyPath) => query.ThenByDescending(Selector<T, TProp>(propertyPath));

		#endregion

		#region Where

		public static IQueryable<T> Where<T>(this IQueryable<T> query, string propertyPath, object filterValue, Comparison comparison = Comparison.Equal) {
			return query.Where(Predicate<T>(propertyPath, filterValue, comparison));
		}

		#endregion

		#region Supports

		private static Type GetPropertyType<T>(string propertyPath) {
			var type = typeof(T);
			foreach ( var propertyName in propertyPath.Split('.') ) {
				type = type.GetProperty(propertyName).PropertyType;
			}
			return type;
		}

		private static Expression<Func<T, TReturn>> Selector<T, TReturn>(string propertyPath) {
			var arg = Expression.Parameter(typeof(T));
			Expression property = null;
			foreach ( var propertyName in propertyPath.Split('.') ) {
				property = Expression.Property(property ?? arg, propertyName);
			}
			return Expression.Lambda<Func<T, TReturn>>(property, arg);
		}

		private static Expression<Func<T, bool>> Predicate<T>(string propertyPath, object filterValue, Comparison comparison) {
			var arg = Expression.Parameter(typeof(T));
			Expression property = null;
			Expression body = null;

			foreach ( var propertyName in propertyPath.Split('.') ) {
				property = Expression.Property(property ?? arg, propertyName);
			}

			var val = Expression.Constant(filterValue, property.Type);
			switch ( comparison ) {
				default:
				case Comparison.Equal:
					body = Expression.Equal(property, val);
					break;
				case Comparison.NotEqual:
					body = Expression.NotEqual(property, val);
					break;
				case Comparison.HasKeyword:
					body = Expression.Call(property, typeof(string).GetMethod(nameof(string.Contains)), val);
					break;
				case Comparison.GreaterThan:
					body = Expression.GreaterThan(property, val);
					break;
				case Comparison.GreaterThanOrEqual:
					body = Expression.GreaterThanOrEqual(property, val);
					break;
				case Comparison.LessThan:
					body = Expression.LessThan(property, val);
					break;
				case Comparison.LessThanOrEqual:
					body = Expression.LessThanOrEqual(property, val);
					break;
			}
			return Expression.Lambda<Func<T, bool>>(body, arg);
		}

		#endregion
	}
}