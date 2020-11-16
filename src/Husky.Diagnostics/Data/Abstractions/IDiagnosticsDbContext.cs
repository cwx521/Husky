using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public interface IDiagnosticsDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<ExceptionLog> ExceptionLogs { get; set; }
		DbSet<RequestLog> RequestLogs { get; set; }
		DbSet<OperationLog> OperationLogs { get; set; }
	}
}
