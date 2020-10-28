using Microsoft.EntityFrameworkCore;

namespace Husky.Diagnostics.Data
{
	public interface IDiagnosticsDbContext
	{
		DbContext Normalize();

		DbSet<ExceptionLog> ExceptionLogs { get; set; }
		DbSet<RequestLog> RequestLogs { get; set; }
		DbSet<OperationLog> OperationLogs { get; set; }
	}
}
