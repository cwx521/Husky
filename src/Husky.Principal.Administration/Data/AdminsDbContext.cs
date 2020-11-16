#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Administration.Data
{
	public class AdminsDbContext : DbContext, IAdminsDbContext
	{
		public AdminsDbContext(DbContextOptions<AdminsDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<Admin> Admins { get; set; }
		public DbSet<AdminRole> AdminRoles { get; set; }
		public DbSet<AdminInRole> AdminInRoles { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomAnnotations(this);
			modelBuilder.OnAdminsDbModelCreating();
		}
	}
}
