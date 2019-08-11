using System;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class DbContextOptionsBuilderExtensions
	{
		public static void Migrate(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			var context = Activator.CreateInstance(contextType, optionsBuilder.Options) as DbContext;
			context.Database.Migrate();
		}
	}
}
