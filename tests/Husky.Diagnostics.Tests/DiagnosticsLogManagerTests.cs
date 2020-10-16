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
			var dbName = $"UnitTest_{nameof(DiagnosticsLogManagerTests)}_{nameof(LogExceptionTest)}";
			var dbBuilder = new DbContextOptionsBuilder<DiagnosticsDbContext>();
			dbBuilder.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog={dbName}; Integrated Security=True");

			using var db = dbBuilder.CreateDbContext();
			db.Database.EnsureDeleted();
			db.Database.Migrate();

			var principal = PrincipalUser.Personate(1, "TestUser", null);

			Exception exception = null;
			try {
				//make an exception
				typeof(string).Name.ToCharArray().GetValue(9999);
			}
			catch ( Exception e ) {
				exception = e;
				db.LogException(e, principal, null);
			}

			Assert.AreEqual(db.ExceptionLogs.Count(), 1);

			var a = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.IsNotNull(a);
			Assert.AreEqual(a.Repeated, 1);
			Assert.AreEqual(a.UserId, principal.Id);
			Assert.AreEqual(a.UserName, principal.DisplayName);
			Assert.AreEqual(a.StackTrace, exception.StackTrace);
			Assert.AreEqual(a.Source, exception.Source);

			//Log the same exception again
			db.LogException(exception, null, null);

			Assert.AreEqual(db.ExceptionLogs.Count(), 1);

			var b = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.AreEqual(b.Repeated, 2);
			Assert.AreEqual(a.Id, b.Id);

			db.Database.EnsureDeleted();
		}
	}
}