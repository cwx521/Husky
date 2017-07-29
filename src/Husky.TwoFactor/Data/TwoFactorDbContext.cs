using Husky.Data;
using Husky.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Husky.TwoFactor.Data
{
	public class TwoFactorDbContext : DbContextBase
	{
		public TwoFactorDbContext(IDatabaseFinder connstr) : base(connstr) {
		}

		public DbSet<TwoFactorCode> TwoFactorCodes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);
		}
	}
}
