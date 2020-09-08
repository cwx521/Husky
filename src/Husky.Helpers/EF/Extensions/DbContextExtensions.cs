using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Husky
{
	public static class DbContextExtensions
	{
		public static EntityEntry<TEntity> AddOrUpdate<TDbContext, TEntity>(this TDbContext context, TEntity entity)
			where TDbContext : DbContext
			where TEntity : class {

			// Find key and Build query
			var entityEntry = context.Entry(entity);
			var keyProperties = entityEntry.Metadata.FindPrimaryKey().Properties;

			IQueryable<TEntity> query = context.Set<TEntity>();
			foreach ( var key in keyProperties ) {
				query = query.Where(key.Name, entityEntry.Property(key.Name).CurrentValue, Comparison.Equal);
			}

			// Add or Update
			var row = query.SingleOrDefault();
			if ( row == null ) {
				context.Add(entity);
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

		public static EntityEntry<TEntity> Update<TDbContext, TEntity>(this TDbContext context, TEntity entity, params string[] updatingFields)
			where TDbContext : DbContext
			where TEntity : class {

			if ( updatingFields.Length < 1 ) {
				throw new ArgumentException(nameof(updatingFields));
			}

			var entityEntry = context.Entry(entity);
			if ( entityEntry.IsKeySet ) {
				throw new ArgumentException($"The Primary Key data of the entity parameter is not set.", nameof(entity));
			}

			// Find key
			var keyProperties = entityEntry.Metadata.FindPrimaryKey().Properties;

			// Find entry from current ChangeTracker
			var query = context.ChangeTracker.Entries<TEntity>().AsQueryable();
			foreach ( var key in keyProperties ) {
				query = query.Where(key.Name, entityEntry.Property(key.Name).CurrentValue, Comparison.Equal);
			} 
			
			//if not, then Attach
			var updating = query.SingleOrDefault() ?? context.Attach(entity);

			// Set IsModified for desired fields
			foreach ( var field in updatingFields ) {
				updating.Property(field).IsModified = true;
			}

			return updating;
		}
	}
}