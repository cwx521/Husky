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
				.AddDbContextPool<IAdminsDbContext, AdminsDbContext>(optionsAction)
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyInjector AddPrincipalAdmin<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IAdminsDbContext {
			husky.Services
				.AddDbContext<IAdminsDbContext, TDbContext>()
				.AddScoped<IPrincipalAdmin, PrincipalAdmin>();
			return husky;
		}

		public static HuskyInjector AddPrincipalAdmin<TImplement>(this HuskyInjector husky, Func<IServiceProvider, TImplement> implementationFactory)
			where TImplement : class, IPrincipalAdmin {
			husky.Services.AddScoped<IPrincipalAdmin, TImplement>(implementationFactory);
			return husky;
		}

		public static HuskyInjector MapPrincipalAdmin<TImplement>(this HuskyInjector husky)
			where TImplement : class, IPrincipalAdmin {
			husky.Services.AddScoped<IPrincipalAdmin, TImplement>();
			return husky;
		}
	}
}
