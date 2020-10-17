using System;
using Husky.Principal.Administration;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddPrincipalAdmin(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<AdminsDbContext>(optionsAction)
				.AddScoped<IAdminsDbContext, AdminsDbContext>()
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyDI AddPrincipalAdmin<TPrincipalAdminImplement>(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction)
			where TPrincipalAdminImplement : class, IPrincipalAdmin {
			husky.Services
				.AddDbContextPool<AdminsDbContext>(optionsAction)
				.AddScoped<IAdminsDbContext, AdminsDbContext>()
				.AddScoped<IPrincipalAdmin, TPrincipalAdminImplement>();
			return husky;
		}

		public static HuskyDI AddPrincipalAdmin<TAdminsDbContextImplement>(this HuskyDI husky)
			where TAdminsDbContextImplement : class, IAdminsDbContext {
			husky.Services
				.AddScoped<IAdminsDbContext, TAdminsDbContextImplement>()
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyDI AddPrincipalAdmin<TAdminsDbContext, TPrincipalAdmin>(this HuskyDI husky)
			where TAdminsDbContext : class, IAdminsDbContext
			where TPrincipalAdmin : class, IPrincipalAdmin {
			husky.Services
				.AddScoped<IAdminsDbContext, TAdminsDbContext>()
				.AddScoped<IPrincipalAdmin, TPrincipalAdmin>();
			return husky;
		}
	}
}
