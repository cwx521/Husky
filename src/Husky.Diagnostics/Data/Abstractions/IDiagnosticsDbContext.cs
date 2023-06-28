using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public interface IDiagnosticsDbContext : IDbContext, IDisposable, IAsyncDisposable
	{
		DbSet<ExceptionLog> ExceptionLogs { get; set; }
		DbSet<RequestLog> RequestLogs { get; set; }
		DbSet<OperationLog> OperationLogs { get; set; }
		DbSet<PageViewLog> PageViewLogs { get; set; }
	}
}
