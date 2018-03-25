using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Diagnostics
{
	public static class ExceptionLogHelper
	{
		public static void Log(this IServiceProvider serviceProvider, Exception e) {
			serviceProvider.LogAsync(e).Wait();
		}

		public static void Log(this Exception e, IServiceProvider serviceProvider) {
			serviceProvider.LogAsync(e).Wait();
		}

		public static async Task LogAsync(this Exception e, IServiceProvider serviceProvider) {
			await serviceProvider.LogAsync(e);
		}

		public static async Task LogAsync(this IServiceProvider serviceProvider, Exception e) {
			using ( var scope = serviceProvider.CreateScope() ) {
				var db = scope.ServiceProvider.GetRequiredService<DiagnosticsDbContext>();
				var principal = scope.ServiceProvider.GetService<IPrincipalUser>();
				var request = scope.ServiceProvider.GetService<IHttpContextAccessor>()?.HttpContext?.Request;

				var log = new ExceptionLog {
					HttpMethod = request?.Method,
					ExceptionType = e.GetType().FullName,
					Message = e.Message,
					Source = e.Source,
					StackTrace = e.StackTrace,
					Url = request?.GetDisplayUrl(),
					UserName = principal?.DisplayName,
					UserAgent = request?.UserAgent()
				};
				log.ComputeMd5Comparison();

				var existedRow = db.ExceptionLogs.FirstOrDefault(x => x.Md5Comparison == log.Md5Comparison);
				if ( existedRow == null ) {
					db.Add(log);
				}
				else {
					existedRow.Count++;
					existedRow.LastTime = DateTime.Now;
				}
				await db.SaveChangesAsync();
			}
		}
	}
}
