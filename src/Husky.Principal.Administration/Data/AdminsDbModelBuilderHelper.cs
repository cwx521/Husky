#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Administration.Data
{
	public static class AdminsDbModelBuilderHelper
	{
		public static void OnAdminsDbModelCreating(this ModelBuilder modelBuilder) {

			//QueryFilters
			modelBuilder.Entity<Admin>().HasQueryFilter(x => x.Status != RowStatus.Deleted);
			modelBuilder.Entity<AdminInRole>().HasQueryFilter(x => x.Admin.Status != RowStatus.Deleted);

			//Indexes
			modelBuilder.Entity<AdminInRole>().HasKey(x => new { x.AdminId, x.RoleId });

			//Relationships
			modelBuilder.Entity<AdminInRole>(adminInRole => {
				adminInRole.HasOne(x => x.Admin).WithMany(x => x.InRoles).HasForeignKey(x => x.AdminId);
				adminInRole.HasOne(x => x.Role).WithMany(x => x.GrantedToAdmins).HasForeignKey(x => x.RoleId);
			});
		}
	}
}
