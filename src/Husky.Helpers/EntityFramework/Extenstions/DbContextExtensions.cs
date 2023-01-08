using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Husky
{
	public static class DbContextExtensions
	{
		public static EntityEntry<TEntity> AddOrUpdate<TDbContext, TEntity>(this TDbContext context, TEntity instance)
			where TDbContext : DbContext
			where TEntity : class {

			// Find key and Build query
			var entityEntry = context.Entry(instance);
			var keyProperties = entityEntry.Metadata.FindPrimaryKey()!.Properties;

			IQueryable<TEntity> query = context.Set<TEntity>();
			foreach (var key in keyProperties) {
				query = query.Where(key.Name, entityEntry.Property(key.Name).CurrentValue!, Comparison.Equal);
			}

			// Add or Update
			var row = query.SingleOrDefault();
			if (row == null) {
				context.Add(instance);
			}
			else {
				var properties = context.Entry(row).Properties;
				foreach (var p in properties) {
					if (p.Metadata.PropertyInfo!.IsDefined(typeof(NeverUpdateAttribute))) {
						continue;
					}
					p.CurrentValue = entityEntry.Property(p.Metadata.Name).CurrentValue;
				}
			}
			return entityEntry;
		}
	}
}