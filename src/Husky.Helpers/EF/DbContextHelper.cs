using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
	public static class DbContextHelper
	{
		public static IQueryable<TEntity> AsNoTracking<TEntity>(this IQueryable<TEntity> query, bool asNoTracking)
			where TEntity : class {

			if ( asNoTracking ) {
				return query.AsNoTracking();
			}
			return query;
		}
	}
}
