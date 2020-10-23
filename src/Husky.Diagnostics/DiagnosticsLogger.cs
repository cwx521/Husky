using System;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Husky.Diagnostics
{
	public sealed class DiagnosticsLogger : IDiagnosticsLogger
	{
		public DiagnosticsLogger(IPrincipalUser? principal, IDiagnosticsDbContext db, IHttpContextAccessor httpContextAccessor) {
			_me = principal;
			_db = db;
			_http = httpContextAccessor.HttpContext;
		}

		internal DiagnosticsLogger(IPrincipalUser principal) {
			_me = principal;
			_db = principal.ServiceProvider.GetRequiredService<IDiagnosticsDbContext>();
			_http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
		}

		private readonly IPrincipalUser? _me;
		private readonly IDiagnosticsDbContext _db;
		private readonly HttpContext _http;

		public void LogException(Exception e) => LogExceptionAsync(e).Wait();
		public async Task LogExceptionAsync(Exception e) => await _db.LogExceptionAsync(e, _http, _me);

		public void LogRequest() => LogRequestAsync().Wait();
		public async Task LogRequestAsync() => await _db.LogRequestAsync(_http, _me);

		public void LogOperation(LogLevel logLevel, string message) => LogOperationAsync(logLevel, message).Wait();
		public async Task LogOperationAsync(LogLevel logLevel, string message) => await _db.LogOperationAsync(_me, logLevel, message);
	}
}
