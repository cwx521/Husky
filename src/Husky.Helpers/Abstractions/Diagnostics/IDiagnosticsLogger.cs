using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics
{
	public interface IDiagnosticsLogger
	{
		Task LogExceptionAsync(Exception e);
		Task LogRequestAsync();
		Task LogOperationAsync(LogLevel logLevel, string message);
	}
}
