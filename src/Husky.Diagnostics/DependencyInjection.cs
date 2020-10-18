using System;
using Husky.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyInjector AddDiagnostics(this HuskyInjector husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<DiagnosticsDbContext>(optionsAction);
			husky.Services.AddScoped<IDiagnosticsDbContext, DiagnosticsDbContext>();
			return husky;
		}

		public static HuskyInjector AddDiagnostics<TDbContext>(this HuskyInjector husky)
			where TDbContext : DbContext, IDiagnosticsDbContext {
			husky.Services.AddScoped<IDiagnosticsDbContext, TDbContext>();
			return husky;
		}
	}
}