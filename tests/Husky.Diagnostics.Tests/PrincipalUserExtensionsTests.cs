using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Diagnostics.Tests
{
	[TestClass()]
	public class PrincipalUserExtensionsTests
	{
		[TestMethod()]
		public void LogExceptionTest() {
			using var testDb = new DbContextOptionsBuilder<DiagnosticsDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			var principal = PrincipalUser.Personate(1, "TestUser", null);

			var exception = new Exception("Oops");
			testDb.LogException(exception, null, null).Wait();
			Assert.AreEqual(1, testDb.ExceptionLogs.Count());

			var findRow = testDb.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(exception.StackTrace, findRow.StackTrace);
			Assert.AreEqual(exception.Source, findRow.Source);

			//Log the same exception again
			testDb.LogException(exception, null, null).Wait();
			Assert.AreEqual(1, testDb.ExceptionLogs.Count());

			findRow = testDb.ExceptionLogs.FirstOrDefault(x => x.Message == exception.Message);
			Assert.AreEqual(2, findRow.Repeated);
			Assert.AreEqual(findRow.Id, findRow.Id);

			//Log the same exception with principal identity this time
			testDb.LogException(exception, null, principal).Wait();
			Assert.AreEqual(2, testDb.ExceptionLogs.Count());

			findRow = testDb.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message && x.UserId == principal.Id);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(principal.Id, findRow.UserId);
			Assert.AreEqual(principal.DisplayName, findRow.UserName);

			//Log a different exception
			exception = new InvalidOperationException("Boom");
			testDb.LogException(exception, null, principal).Wait();
			Assert.AreEqual(3, testDb.ExceptionLogs.Count());

			findRow = testDb.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(exception.Source, findRow.Source);

			testDb.Database.EnsureCreated();
		}

		[TestMethod()]
		public void LogOperationTest() {
			using var testDb = new DbContextOptionsBuilder<DiagnosticsDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			var principal = PrincipalUser.Personate(1, "TestUser", null);

			testDb.LogOperation(principal, "UnitTest", LogLevel.Trace).Wait();
			Assert.AreEqual(1, testDb.OperationLogs.Count());

			var findRow = testDb.OperationLogs.FirstOrDefault(x => x.Message == "UnitTest");
			Assert.IsNotNull(findRow);
			Assert.AreEqual(1, findRow.Repeated);
			Assert.AreEqual(LogLevel.Trace, findRow.LogLevel);
			Assert.AreEqual(principal.Id, findRow.UserId);
			Assert.AreEqual(principal.DisplayName, findRow.UserName);

			//log the same message one more time

			testDb.LogOperation(principal, "UnitTest", LogLevel.Trace).Wait();
			Assert.AreEqual(1, testDb.OperationLogs.Count());

			findRow = testDb.OperationLogs.FirstOrDefault(x => x.Message == "UnitTest");
			Assert.AreEqual(2, findRow.Repeated);

			testDb.Database.EnsureDeleted();
		}
	}
}