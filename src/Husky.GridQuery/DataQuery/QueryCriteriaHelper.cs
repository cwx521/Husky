using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Husky.GridQuery
{
	public static class QueryCriteriaHelper
	{
		public static IQueryable<T> ApplyAllFilters<T>(this IQueryable<T> query, QueryCriteria criteria) => query.ApplyPreFilters(criteria).ApplyPostFilters(criteria);
		public static IQueryable<T> ApplyPreFilters<T>(this IQueryable<T> query, QueryCriteria criteria) => query.ApplyFilters(criteria.PreFilters, true);
		public static IQueryable<T> ApplyPostFilters<T>(this IQueryable<T> query, QueryCriteria criteria) => query.ApplyFilters(criteria.PostFilters, false);

		private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<QueryFilter> filters, bool throwExceptionWhenFilterNotApplicable) {
			foreach (var filter in filters) {
				if (!typeof(T).GetProperties().Any(x => string.Compare(filter.Field, x.Name, true) == 0)) {
					if (throwExceptionWhenFilterNotApplicable) {
						throw new InvalidProgramException(
							$"The filter '{JsonSerializer.Serialize(filter)}' is not applicable, " +
							$"field '{filter.Field}' does not exist."
						);
					}
					continue;
				}
				if (filter.Value == null) {
					continue;
				}
				query = query.Where(filter.Field, filter.Value, filter.Operator.Equality());
			}
			return query;
		}

		public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, QueryCriteria criteria) {
			var sortBy = criteria.SortBy;
			var sortDirection = criteria.SortDirection;

			if (sortBy == null || typeof(T).GetProperty(sortBy, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance) == null) {
				var property = typeof(T).GetProperties().FirstOrDefault(p => p.IsDefined(typeof(SortAttribute)));
				if (property != null) {
					var attribute = property.GetCustomAttribute<SortAttribute>()!;
					sortBy = property.Name;
					sortDirection = attribute.SortDirection;
				}
			}
			if (sortBy == null) {
				return query;
			}
			return query.OrderBy(sortBy, sortDirection);
		}

		public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, QueryCriteria criteria) {
			if (!criteria.PaginationEnabled) {
				return query;
			}
			return query.Skip(criteria.Offset).Take(criteria.Limit);
		}

		public static QueryResult<T> Apply<T>(this IQueryable<T> query, QueryCriteria criteria) {
			var filteredQuery = query.ApplyPreFilters(criteria).ApplyPostFilters(criteria);
			return new() {
				totalCount = filteredQuery.Count(),
				data = filteredQuery.ApplySort(criteria).ApplyPagination(criteria).ToList()
			};
		}
	}
}