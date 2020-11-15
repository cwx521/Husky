using System;
using Husky.Principal.Administration;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddPrincipalAdmin(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<IAdminsDbContext, AdminsDbContext>(optionsAction);
			husky.Services.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyServiceHub AddPrincipalAdmin<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, IAdminsDbContext {
			husky.Services.AddDbContext<IAdminsDbContext, TDbContext>();
			husky.Services.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}
	}
}
