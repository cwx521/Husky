using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public interface IDiagnosticsDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		public DbSet<ExceptionLog> ExceptionLogs { get; set; }
		public DbSet<RequestLog> RequestLogs { get; set; }
		public DbSet<OperationLog> OperationLogs { get; set; }
	}
}
