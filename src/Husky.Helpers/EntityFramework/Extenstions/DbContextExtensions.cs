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
			var keyProperties = entityEntry.Metadata.FindPrimaryKey().Properties;

			IQueryable<TEntity> query = context.Set<TEntity>();
			foreach ( var key in keyProperties ) {
				query = query.Where(key.Name, entityEntry.Property(key.Name).CurrentValue, Comparison.Equal);
			}

			// Add or Update
			var row = query.SingleOrDefault();
			if ( row == null ) {
				context.Add(instance);
			}
			else {
				var properties = context.Entry(row).Properties;
				foreach ( var p in properties ) {
					if ( p.Metadata.PropertyInfo.IsDefined(typeof(NeverUpdateAttribute)) ) {
						continue;
					}
					p.CurrentValue = entityEntry.Property(p.Metadata.Name).CurrentValue;
				}
			}
			return entityEntry;
		}

		//public static EntityEntry<TEntity> Update<TDbContext, TEntity, TProperty>(this TDbContext context, TEntity instance, Expression<Func<TEntity, TProperty>> propertyToUpdate)
		//	where TDbContext : DbContext
		//	where TEntity : class {
		//	var propertyName = propertyToUpdate.Body.ToString().RightBy(".", true)!;
		//	return Update(context, instance, propertyName);
		//}

		//public static EntityEntry<TEntity> Update<TDbContext, TEntity>(this TDbContext context, TEntity instance, params string[] updatingFields)
		//	where TDbContext : DbContext
		//	where TEntity : class {

		//	if ( updatingFields.Length < 1 ) {
		//		throw new ArgumentException(nameof(updatingFields));
		//	}

		//	var entityEntry = context.Entry(instance);
		//	if ( entityEntry.IsKeySet ) {
		//		throw new ArgumentException($"The Primary Key data of the entity parameter is not set.", nameof(instance));
		//	}

		//	// Find key
		//	var keyProperties = entityEntry.Metadata.FindPrimaryKey().Properties;

		//	// Find entry from current ChangeTracker
		//	var query = context.ChangeTracker.Entries<TEntity>().AsQueryable();
		//	foreach ( var key in keyProperties ) {
		//		query = query.Where(key.Name, entityEntry.Property(key.Name).CurrentValue, Comparison.Equal);
		//	}

		//	//if not, then Attach
		//	var updating = query.SingleOrDefault() ?? context.Attach(instance);

		//	// Set IsModified for desired fields
		//	foreach ( var field in updatingFields ) {
		//		updating.Property(field).IsModified = true;
		//	}

		//	return updating;
		//}
	}
}