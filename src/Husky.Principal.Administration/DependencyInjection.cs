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

		public static HuskyDI AddPrincipalAdmin<TDbContext>(this HuskyDI husky)
			where TDbContext : DbContext, IAdminsDbContext {
			husky.Services
				.AddScoped<IAdminsDbContext, TDbContext>()
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyDI MapPrincipalAdmin<TPrincipalAdminImplement>(this HuskyDI husky)
			where TPrincipalAdminImplement : class, IPrincipalAdmin {
			husky.Services.AddScoped<IPrincipalAdmin, TPrincipalAdminImplement>();
			return husky;
		}

		public static HuskyDI MapPrincipalAdmin<TPrincipalAdminImplement>(this HuskyDI husky, Func<IServiceProvider, TPrincipalAdminImplement> implementationFactory)
			where TPrincipalAdminImplement : class, IPrincipalAdmin {
			husky.Services.AddScoped<IPrincipalAdmin, TPrincipalAdminImplement>(implementationFactory);
			return husky;
		}
	}
}
