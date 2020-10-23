using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class DbContextOptionsBuilderExtensions
	{
		private static readonly object _lock = new object();
		private static readonly List<Type> _migrated = new List<Type>();

		public static DbContext CreateDbContext(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			var context = (DbContext)Activator.CreateInstance(contextType, optionsBuilder.Options)!;
			return context;
		}

		public static TDbContext CreateDbContext<TDbContext>(this DbContextOptionsBuilder<TDbContext> optionsBuilder)
			where TDbContext : DbContext {
			var contextType = optionsBuilder.Options.ContextType;
			var context = (TDbContext)Activator.CreateInstance(contextType, optionsBuilder.Options)!;
			return context;
		}

		public static DbContext Migrate(
			this DbContextOptionsBuilder optionsBuilder,
			bool ensureDeletedThenRecreateIfExists = false,
			bool doubleConfirmIfYouWantToEnsureDeletedThenRecreate = false) {

			var contextType = optionsBuilder.Options.ContextType;
			var context = optionsBuilder.CreateDbContext();

			if ( ensureDeletedThenRecreateIfExists && doubleConfirmIfYouWantToEnsureDeletedThenRecreate ) {
				context.Database.EnsureDeleted();
				_migrated.Remove(contextType);
			}

			if ( !_migrated.Contains(contextType) ) {
				lock ( _lock ) {
					_migrated.Add(contextType);
				}
				context.Database.Migrate();
			}
			return context;
		}

		public static TDbContext Migrate<TDbContext>(
			this DbContextOptionsBuilder<TDbContext> optionsBuilder,
			bool ensureDeletedThenRecreateIfExists = false,
			bool doubleConfirmIfYouWantToEnsureDeletedThenRecreate = false)
			where TDbContext : DbContext {

			return (TDbContext)((DbContextOptionsBuilder)optionsBuilder).Migrate(ensureDeletedThenRecreateIfExists, doubleConfirmIfYouWantToEnsureDeletedThenRecreate);
		}
	}
}
