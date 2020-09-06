using System;
using Husky.TwoFactor;
using Husky.TwoFactor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddDiagnostics(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services
				.AddDbContextPool<TwoFactorDbContext>(optionsAction)
				.AddScoped<TwoFactorManager>();

			return husky;
		}
	}
}
