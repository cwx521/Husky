#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

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
