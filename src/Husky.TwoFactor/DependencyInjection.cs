using System;
using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddTwoFactor(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<ITwoFactorDbContext, TwoFactorDbContext>(optionsAction)
				.AddScoped<ITwoFactorManager, TwoFactorManager>();
			return husky;
		}

		public static HuskyServiceHub AddTwoFactor<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, ITwoFactorDbContext {
			husky.Services
				.AddDbContext<ITwoFactorDbContext, TDbContext>()
				.AddScoped<ITwoFactorManager, TwoFactorManager>();
			return husky;
		}
	}
}
