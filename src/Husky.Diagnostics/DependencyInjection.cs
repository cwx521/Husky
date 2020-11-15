using System;
using Husky.Diagnostics;
using Husky.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddDiagnostics(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<IDiagnosticsDbContext, DiagnosticsDbContext>(optionsAction);
			husky.Services.AddScoped<IDiagnosticsLogger, DiagnosticsLogger>();
			return husky;
		}

		public static HuskyServiceHub AddDiagnostics<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, IDiagnosticsDbContext {
			husky.Services.AddDbContext<IDiagnosticsDbContext, TDbContext>();
			husky.Services.AddScoped<IDiagnosticsLogger, DiagnosticsLogger>();
			return husky;
		}
	}
}