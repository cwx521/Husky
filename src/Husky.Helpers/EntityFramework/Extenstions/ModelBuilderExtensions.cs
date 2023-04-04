using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Husky
{
	public static class ModelBuilderExtensions
	{
		public static ModelBuilder ApplyHuskyAnnotations(this ModelBuilder modelBuilder, DbContext context) {
			if ( !context.Database.IsRelational() ) {
				return modelBuilder;
			}

			modelBuilder.Model.GetEntityTypes().AsParallel().ForAll(entity => {

				var entityBuilder = modelBuilder.Entity(entity.ClrType);

				foreach ( var p in entity.ClrType.GetProperties() ) {
					if ( p.GetCustomAttribute<DefaultValueSqlAttribute>() is DefaultValueSqlAttribute valueSql && context.Database.IsSqlServer() ) {
						entityBuilder.Property(p.Name).HasDefaultValueSql(valueSql.Sql);
					}
					if ( p.GetCustomAttribute<EnableIndexAttribute>() is EnableIndexAttribute index ) {
						entityBuilder.HasIndex(p.Name).IsUnique(index.IsUnique).IsClustered(index.IsClustered);
					}
					if ( p.GetCustomAttribute<UniqueAttribute>() != null ) {
						entityBuilder.HasIndex(p.Name).IsUnique();
					}
				}
			});

			return modelBuilder;
		}
	}
}
