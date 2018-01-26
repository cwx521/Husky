using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class DbContextOptionsBuilderExtensions
	{
		public static void Migrate(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			var constructor = contextType.GetTypeInfo().GetConstructor(new[] { typeof(DbContextOptions) });
			var context = Activator.CreateInstance(contextType, optionsBuilder.Options) as DbContext;
			context.Database.Migrate();
		}
	}
}
