using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Husky.Principal;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Diagnostics.Tests
{
	[TestClass()]
	public class DiagnosticsLogManagerTests
	{
		[TestMethod()]
		public void LogExceptionTest() {
			using var testDb = new DbContextOptionsBuilder<DiagnosticsDbContext>().UseInMemoryDatabase("UnitTest").CreateDbContext();
			var principal = PrincipalUser.Personate(1, "TestUser", null);

			Exception exception = null;
			try {
				//make an exception
				typeof(string).Name.ToCharArray().GetValue(9999);
			}
			catch ( Exception e ) {
				exception = e;
				testDb.LogException(e, principal, null).Wait();
			}

			Assert.AreEqual(testDb.ExceptionLogs.Count(), 1);

			var a = testDb.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.IsNotNull(a);
			Assert.AreEqual(a.Repeated, 1);
			Assert.AreEqual(a.UserId, principal.Id);
			Assert.AreEqual(a.UserName, principal.DisplayName);
			Assert.AreEqual(a.StackTrace, exception.StackTrace);
			Assert.AreEqual(a.Source, exception.Source);

			//Log the same exception again
			testDb.LogException(exception, null, null).Wait();

			Assert.AreEqual(testDb.ExceptionLogs.Count(), 1);

			var b = testDb.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.AreEqual(b.Repeated, 2);
			Assert.AreEqual(a.Id, b.Id);

			testDb.Database.EnsureDeleted();
		}
	}
}