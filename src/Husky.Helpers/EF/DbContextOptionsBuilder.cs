using System;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class DbContextOptionsBuilderExtensions
	{
		public static DbContext CreateDbContext(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			var context = Activator.CreateInstance(contextType, optionsBuilder.Options) as DbContext;
			return context;
		}
		public static T CreateDbContext<T>(this DbContextOptionsBuilder<T> optionsBuilder) where T : DbContext {
			var contextType = optionsBuilder.Options.ContextType;
			var context = Activator.CreateInstance(contextType, optionsBuilder.Options);
			return context as T;
		}
	}
}
