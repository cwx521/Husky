using System;
using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddTwoFactor(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<TwoFactorDbContext>(optionsAction)
				.AddScoped<ITwoFactorDbContext, TwoFactorDbContext>()
				.AddScoped<ITwoFactorManager, TwoFactorManager>();

			return husky;
		}

		public static HuskyDI AddTwoFactor<TDbContext>(this HuskyDI husky)
			where TDbContext : DbContext, ITwoFactorDbContext {
			husky.Services
				.AddScoped<ITwoFactorDbContext, TDbContext>()
				.AddScoped<ITwoFactorManager, TwoFactorManager>();

			return husky;
		}

		public static HuskyDI MapTwoFactor<TTwoFactorImplement>(this HuskyDI husky)
			where TTwoFactorImplement : class, ITwoFactorManager {
			husky.Services.AddScoped<ITwoFactorManager, TTwoFactorImplement>();
			return husky;
		}

		public static HuskyDI MapTwoFactor<TTwoFactorImplement>(this HuskyDI husky, Func<IServiceProvider, TTwoFactorImplement> implementationFactory)
			where TTwoFactorImplement : class, ITwoFactorManager {
			husky.Services.AddScoped<ITwoFactorManager, TTwoFactorImplement>(implementationFactory);
			return husky;
		}
	}
}
