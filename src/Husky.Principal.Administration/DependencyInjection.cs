using System;
using Husky.Principal.Administration;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddPrincipalAdmin(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<AdminsDbContext>(optionsAction)
				.AddScoped<IAdminsDbContext, AdminsDbContext>()
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyInjector AddPrincipalAdmin<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IAdminsDbContext {
			husky.Services
				.AddScoped<IAdminsDbContext, TDbContext>()
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyInjector MapPrincipalAdmin<TPrincipalAdminImplement>(this HuskyInjector husky)
			where TPrincipalAdminImplement : class, IPrincipalAdmin {
			husky.Services.AddScoped<IPrincipalAdmin, TPrincipalAdminImplement>();
			return husky;
		}

		public static HuskyInjector MapPrincipalAdmin<TPrincipalAdminImplement>(this HuskyInjector husky, Func<IServiceProvider, TPrincipalAdminImplement> implementationFactory)
			where TPrincipalAdminImplement : class, IPrincipalAdmin {
			husky.Services.AddScoped<IPrincipalAdmin, TPrincipalAdminImplement>(implementationFactory);
			return husky;
		}
	}
}
