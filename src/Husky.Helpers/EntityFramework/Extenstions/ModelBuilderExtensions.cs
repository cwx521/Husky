using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class ModelBuilderExtensions
	{
		public static ModelBuilder ApplyAdditionalCustomizedAnnotations(this ModelBuilder modelBuilder) {
			modelBuilder.Model.GetEntityTypes().AsParallel().ForAll(entity => {

				var entityBuilder = modelBuilder.Entity(entity.ClrType);
				var compositeUniqueProperties = new List<string>();

				foreach ( var p in entity.ClrType.GetProperties() ) {
					if ( p.GetCustomAttribute<DefaultValueSqlAttribute>() is DefaultValueSqlAttribute valueSql ) {
						entityBuilder.Property(p.Name).HasDefaultValueSql(valueSql.Sql);
					}
					if ( p.GetCustomAttribute<IndexAttribute>() is IndexAttribute index ) {
						entityBuilder.HasIndex(p.Name).IsUnique(index.IsUnique).IsClustered(index.IsClustered);
					}
					if ( p.GetCustomAttribute<UniqueAttribute>() != null ) {
						entityBuilder.HasIndex(p.Name).IsUnique();
					}
					if ( p.GetCustomAttribute<UniqueCompositeAttribute>() != null ) {
						compositeUniqueProperties.Add(p.Name);
					}
				}

				if ( compositeUniqueProperties.Count != 0 ) {
					entityBuilder.HasIndex(compositeUniqueProperties.ToArray()).IsUnique(true);
				}
			});

			return modelBuilder;
		}
	}
}
