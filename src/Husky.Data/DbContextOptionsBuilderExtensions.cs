using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Husky.Data
{
	public static class DbContextOptionsBuilderExtensions
	{
		public static async void Migrate<T>(this DbContextOptionsBuilder optionsBuilder) where T : DbContext {
			var constructor = typeof(T).GetTypeInfo().GetConstructor(new[] { typeof(DbContextOptions<T>) });
			var context = Activator.CreateInstance(typeof(T), optionsBuilder.Options) as T;
			await context.Database.MigrateAsync();
		}

		public static async void Migrate(this DbContextOptionsBuilder optionsBuilder) {
			var contextType = optionsBuilder.Options.ContextType;
			var constructor = contextType.GetTypeInfo().GetConstructor(new[] { typeof(DbContextOptions) });
			var context = Activator.CreateInstance(contextType, optionsBuilder.Options) as DbContext;
			await context.Database.MigrateAsync();
		}
	}
}
