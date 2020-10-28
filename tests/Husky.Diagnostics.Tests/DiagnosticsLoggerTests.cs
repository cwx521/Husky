using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Diagnostics.Tests
{
	[TestClass()]
	public class DiagnosticsLoggerTests
	{
		private IServiceProvider BuildServiceProvider() {
			var services = new ServiceCollection();
			services.AddScoped<IHttpContextAccessor>(x => new HttpContextAccessor { HttpContext = new DefaultHttpContext() });
			services.AddDbContext<IDiagnosticsDbContext, DiagnosticsDbContext>(x => x.UseInMemoryDatabase("UnitTest"));

			var serviceProvider = services.BuildServiceProvider();
			var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
			httpContextAccessor.HttpContext.RequestServices = serviceProvider;

			return serviceProvider;
		}

		[TestMethod()]
		public async Task LogExceptionAsyncTestAsync() {
			var serviceProvider = BuildServiceProvider();
			var db = serviceProvider.GetRequiredService<IDiagnosticsDbContext>();
			var http = serviceProvider.GetRequiredService<IHttpContextAccessor>();

			var anonymous = PrincipalUser.Personate(0, "Anonymous", serviceProvider);
			var logger = new DiagnosticsLogger(anonymous, db, http);

			var exception = new Exception("Oops");

			//insert exception log should be successful, the PrincipalUser is anonymous
			await logger.LogExceptionAsync(exception);
			Assert.AreEqual(1, db.ExceptionLogs.Count());

			//exception data should be written to the fields
			var findRow = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			var rowId = findRow.Id;
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(exception.GetType().Name, findRow.ExceptionType);
			Assert.AreEqual(exception.StackTrace, findRow.StackTrace);
			Assert.AreEqual(exception.Source, findRow.Source);
			Assert.AreEqual(anonymous.Id, findRow.UserId);
			Assert.AreEqual(anonymous.AnonymousId, findRow.AnonymousId);
			Assert.AreEqual(anonymous.DisplayName, findRow.UserName);

			//log the same exception again, should be still only 1 exception log record
			await logger.LogExceptionAsync(exception);
			Assert.AreEqual(1, db.ExceptionLogs.Count());

			//find back the row, the Repeated value should be 2 now
			findRow = db.ExceptionLogs.Find(rowId);
			Assert.AreEqual(2, findRow.Repeated);

			//log the same exception with a known user this time
			var someone = PrincipalUser.Personate(1, "Someone", serviceProvider);
			logger = new DiagnosticsLogger(someone, db, http);

			//a new row should be inserted and there should be 2 rows in total
			await logger.LogExceptionAsync(exception);
			findRow = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message && x.UserId == someone.Id);
			Assert.AreEqual(2, db.ExceptionLogs.Count());
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(someone.Id, findRow.UserId);
			Assert.AreEqual(someone.AnonymousId, findRow.AnonymousId);
			Assert.AreEqual(someone.DisplayName, findRow.UserName);

			//log a different exception
			exception = new InvalidOperationException("Boom");
			await logger.LogExceptionAsync(exception);

			//another new row should be inserted and there should be 3 rows in total
			findRow = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.AreEqual(3, db.ExceptionLogs.Count());
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(exception.Source, findRow.Source);
			Assert.AreEqual(exception.GetType().Name, findRow.ExceptionType);
			Assert.AreEqual(exception.StackTrace, findRow.StackTrace);
			Assert.AreEqual(exception.Source, findRow.Source);
			Assert.AreEqual(someone.Id, findRow.UserId);
			Assert.AreEqual(someone.AnonymousId, findRow.AnonymousId);
			Assert.AreEqual(someone.DisplayName, findRow.UserName);

			db.Normalize().Database.EnsureDeleted();
		}

		[TestMethod()]
		public async Task LogOperationAsyncTestAsync() {
			var serviceProvider = BuildServiceProvider();
			var db = serviceProvider.GetRequiredService<IDiagnosticsDbContext>();
			var http = serviceProvider.GetRequiredService<IHttpContextAccessor>();
			var principal = PrincipalUser.Personate(1, "Someone", serviceProvider);

			var logger = new DiagnosticsLogger(principal, db, http);

			//log an operation
			await logger.LogOperationAsync(LogLevel.Trace, "UnitTest");
			Assert.AreEqual(1, db.OperationLogs.Count());

			var findRow = db.OperationLogs.FirstOrDefault(x => x.Message == "UnitTest");
			var rowId = findRow.Id;
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(LogLevel.Trace, findRow.LogLevel);
			Assert.AreEqual(principal.Id, findRow.UserId);
			Assert.AreEqual(principal.AnonymousId, findRow.AnonymousId);
			Assert.AreEqual(principal.DisplayName, findRow.UserName);

			//log the same message one more time, should be still only 1 row
			await logger.LogOperationAsync(LogLevel.Trace, "UnitTest");
			Assert.AreEqual(1, db.OperationLogs.Count());

			//find back the row, the Repeated value should be 2 now
			findRow = db.OperationLogs.Find(rowId);
			Assert.AreEqual(2, findRow.Repeated);

			//log some different operations
			await logger.LogOperationAsync(LogLevel.Warning, "Make a difference");
			await logger.LogOperationAsync(LogLevel.Warning, "Again");
			await logger.LogOperationAsync(LogLevel.Warning, "One more time");
			Assert.AreEqual(4, db.OperationLogs.Count());

			db.Normalize().Database.EnsureDeleted();
		}
	}
}