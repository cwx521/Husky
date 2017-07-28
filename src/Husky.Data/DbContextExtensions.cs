using System;
using System.Linq;
using Husky.Sugar;
using Microsoft.EntityFrameworkCore;

namespace Husky.Data
{
	public static class DbContextExtensions
	{
		public static string LocalIntegratedSecurityConnectionString(this DbContext context, string databaseName) {
			if ( string.IsNullOrWhiteSpace(databaseName) ) {
				throw new ArgumentNullException(nameof(databaseName));
			}
			return $"Data Source=localhost;Initial Catalog={databaseName};Integrated Security=True";
		}

		public static void Update<TDbContext, TEntity>(this TDbContext context, TEntity entity, params string[] updatingFields)
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
			var import = context.Entry(entity);
			var keyProperties = import.Metadata.FindPrimaryKey().Properties;

			if ( import.IsKeySet ) {
				throw new ArgumentException($"The Primary Key data of the entity parameter is not set.", nameof(entity));
			}

			// Find entry from ChangeTracker; if not found then Attach new with IsKeySet
			var query = context.ChangeTracker.Entries<TEntity>();
			foreach ( var key in keyProperties ) {
				query = query.AsQueryable().Where(key.Name, import.Property(key.Name).CurrentValue, Comparison.Equal);
			}
			var updating = query.SingleOrDefault() ?? context.Attach(entity);

			// Set IsModified for desired fields
			foreach ( var field in updatingFields ) {
				updating.Property(field).IsModified = true;
			}
		}
	}
}