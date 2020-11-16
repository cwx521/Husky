using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Administration.Data
{
	public interface IAdminsDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<Admin> Admins { get; set; }
		DbSet<AdminRole> AdminRoles { get; set; }
		DbSet<AdminInRole> AdminInRoles { get; set; }
	}
}
