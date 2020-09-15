using System;
using Husky.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyDI AddDiagnostics(this HuskyDI husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<DiagnosticsDbContext>(optionsAction);
			return husky;
		}
	}
}