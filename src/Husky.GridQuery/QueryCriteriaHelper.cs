using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Husky.GridQuery
{
	public static class QueryCriteriaHelper
	{
		public static IQueryable<T> ApplyPreFilters<T>(this IQueryable<T> query, QueryCriteria criteria) => query.ApplyFilters(criteria.PreFilters, true);
		public static IQueryable<T> ApplyPostFilters<T>(this IQueryable<T> query, QueryCriteria criteria) => query.ApplyFilters(criteria.PostFilters, false);
		private static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, List<QueryFilter> filters, bool throwExceptionWhenFilterIsNotApplicable) {
			foreach ( var filter in filters ) {
				if ( !typeof(T).GetProperties().Any(x => string.Compare(filter.Field, x.Name, true) == 0) ) {
					if ( throwExceptionWhenFilterIsNotApplicable ) {
						throw new InvalidProgramException($"The PreFilter on column '{JsonConvert.SerializeObject(filter)}' is not applicable.");
					}
					continue;
				}
				if ( filter.Value == null || filter.Value.ToString().Length == 0 ) {
					continue;
				}
				query = query.Where(filter.Field, filter.Value, filter.Operator.Equality());
			}
			return query;
		}

		public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, QueryCriteria criteria) {
			var sortBy = criteria.SortBy;
			var sortDirection = criteria.SortDirection;

			if ( sortBy == null || typeof(T).GetProperty(sortBy, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance) == null ) {
				var property = typeof(T).GetProperties().FirstOrDefault(p => p.IsDefined(typeof(SortAttribute)));
				if ( property != null ) {
					var attribute = property.GetCustomAttribute<SortAttribute>();
					sortBy = property.Name;
					sortDirection = attribute.SortDirection;
				}
			}
			if ( sortBy == null ) {
				return query;
			}
			return query.OrderBy(sortBy, sortDirection);
		}

		public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, QueryCriteria criteria) {
			if ( !criteria.PaginationEnabled ) {
				return query;
			}
			return query.Skip(criteria.Offset).Take(criteria.Limit);
		}

		public static JsonResult Apply<T>(this IQueryable<T> query, QueryCriteria criteria) {
			var q = query.ApplyPreFilters(criteria).ApplyPostFilters(criteria);
			var data = new {
				TotalCount = q.Count(),
				Data = q.ApplySort(criteria).ApplyPagination(criteria).ToList()
			};
			return new JsonResult(data);
		}
	}
}