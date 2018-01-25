using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Husky.AspNetCore
{
	public static class DbContextExtensions
	{
		public static EntityEntry<TEntity> AddOrUpdate<TDbContext, TEntity>(this TDbContext context, TEntity entity)
			where TDbContext : DbContext
			where TEntity : class {

			if ( context == null ) {
				throw new ArgumentNullException(nameof(context));
			}
			if ( entity == null ) {
				throw new ArgumentNullException(nameof(entity));
			}

			// Find keyand Build query
			var importing = context.Entry(entity);
			var keyProperties = importing.Metadata.FindPrimaryKey().Properties;

			IQueryable<TEntity> query = context.Set<TEntity>();
			foreach ( var key in keyProperties ) {
				query = query.Where(key.Name, importing.Property(key.Name).CurrentValue, Comparison.Equal);
			}

			// Add or Update
			var row = query.SingleOrDefault();
			if ( row == null ) {
				context.Add(entity);
			}
			else {
				var updating = context.Entry(row);
				foreach ( var p in updating.Properties ) {
					p.CurrentValue = importing.Property(p.Metadata.Name).CurrentValue;
				}
			}
			return importing;
		}

		public static EntityEntry<TEntity> Update<TDbContext, TEntity>(this TDbContext context, TEntity entity, params string[] updatingFields)
			where TDbContext : DbContext
			where TEntity : class {

			if ( context == null ) {
				throw new ArgumentNullException(nameof(context));
			}
			if ( entity == null ) {
				throw new ArgumentNullException(nameof(entity));
			}
			if ( updatingFields?.Length < 1 ) {
				throw new ArgumentNullException(nameof(updatingFields));
			}

			// Find key
			var tempEntry = context.Entry(entity);
			var keyProperties = tempEntry.Metadata.FindPrimaryKey().Properties;

			if ( tempEntry.IsKeySet ) {
				throw new ArgumentException($"The Primary Key data of the entity parameter is not set.", nameof(entity));
			}

			// Find entry from ChangeTracker; if not found then Attach new with IsKeySet
			var query = context.ChangeTracker.Entries<TEntity>();
			foreach ( var key in keyProperties ) {
				query = query.AsQueryable().Where(key.Name, tempEntry.Property(key.Name).CurrentValue, Comparison.Equal);
			}
			var updating = query.SingleOrDefault() ?? context.Attach(entity);

			// Set IsModified for desired fields
			foreach ( var field in updatingFields ) {
				updating.Property(field).IsModified = true;
			}

			return updating;
		}
	}
}