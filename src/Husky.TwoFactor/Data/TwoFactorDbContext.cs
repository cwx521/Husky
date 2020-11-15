#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor.Data
{
	public class TwoFactorDbContext : DbContext, ITwoFactorDbContext
	{
		public TwoFactorDbContext(DbContextOptions<TwoFactorDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomizedSqlServerAnnotations(this);
		}
	}
}
