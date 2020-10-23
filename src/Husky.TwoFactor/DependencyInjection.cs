using System;
using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddTwoFactor(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<ITwoFactorDbContext, TwoFactorDbContext>(optionsAction)
				.AddScoped<ITwoFactorManager, TwoFactorManager>();
			return husky;
		}

		public static HuskyInjector AddTwoFactor<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, ITwoFactorDbContext {
			husky.Services
				.AddDbContext<ITwoFactorDbContext, TDbContext>()
				.AddScoped<ITwoFactorManager, TwoFactorManager>();
			return husky;
		}
	}
}
