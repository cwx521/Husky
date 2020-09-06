using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class DbContextOptionsBuilderExtensions
	{
		static readonly List<Type> _migrated = new List<Type>();

		public static DbContext CreateDbContext(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			var context = Activator.CreateInstance(contextType, optionsBuilder.Options) as DbContext;
			return context!;
		}

		public static void Migrate(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			if ( !_migrated.Contains(contextType) ) {
				optionsBuilder.CreateDbContext().Database.Migrate();
				_migrated.Add(contextType);
			}
		}
	}
}
