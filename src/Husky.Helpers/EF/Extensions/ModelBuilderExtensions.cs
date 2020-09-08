using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Husky.EF
{
	public static class ModelBuilderExtensions
	{
		public static ModelBuilder ApplyHuskyAnnotations(this ModelBuilder modelBuilder) {

			modelBuilder.Model.GetEntityTypes().AsParallel().ForAll(entity => {
				var entityBuilder = modelBuilder.Entity(entity.ClrType);
				var compositeUniqueProperties = new List<string>();

				entity.ClrType.GetProperties().AsParallel().ForAll(p => {
					var valueSql = p.GetCustomAttribute<DefaultValueSqlAttribute>();
					if ( valueSql != null ) {
						entityBuilder.Property(p.Name).HasDefaultValueSql(valueSql.Sql);
					}

					var index = p.GetCustomAttribute<IndexAttribute>();
					if ( index != null ) {
						entityBuilder.HasIndex(p.Name).IsUnique(index.IsUnique).IsClustered(index.IsClustered);
					}

					if ( p.GetCustomAttribute<CompositeUniqueAttribute>() != null ) {
						compositeUniqueProperties.Add(p.Name);
					}
				});

				if ( compositeUniqueProperties.Count != 0 ) {
					entityBuilder.HasIndex(compositeUniqueProperties.ToArray()).IsUnique(true);
				}
			});

			return modelBuilder;
		}
	}
}
