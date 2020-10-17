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
				.AddScoped<TwoFactorManager>();

			return husky;
		}

		public static HuskyDI AddTwoFactor<TTwoFactorDbContextImplement>(this HuskyDI husky)
			where TTwoFactorDbContextImplement : class, ITwoFactorDbContext {
			husky.Services
				.AddScoped<ITwoFactorDbContext, TTwoFactorDbContextImplement>()
				.AddScoped<TwoFactorManager>();

			return husky;
		}
	}
}
