using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Administration.Data
{
	public interface IAdminsDbContext
	{
		DbContext Normalize();

		DbSet<Admin> Admins { get; set; }
		DbSet<AdminRole> AdminRoles { get; set; }
		DbSet<AdminInRole> AdminInRoles { get; set; }
	}
}
