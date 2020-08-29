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
	}
}
