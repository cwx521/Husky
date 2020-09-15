using System;
using System.Linq;
using Husky.Diagnostics.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Diagnostics.Tests
{
	[TestClass()]
	public class ExceptionLogHelperTests
	{
		[TestMethod()]
		public void LogTest() {
			var dbName = $"UnitTest_{nameof(ExceptionLogHelperTests)}_{nameof(LogTest)}";
			var dbBuilder = new DbContextOptionsBuilder<DiagnosticsDbContext>();
			dbBuilder.UseSqlServer($"Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog={dbName}; Integrated Security=True");

			using var db = dbBuilder.CreateDbContext();
			db.Database.EnsureDeleted();
			db.Database.Migrate();

			Exception exception = null;
			try {
				//make an exception
				typeof(string).Name.ToCharArray().GetValue(9999);
			}
			catch ( Exception e ) {
				exception = e;
				db.Log(e, null, null);
			}

			Assert.AreEqual(db.ExceptionLogs.Count(), 1);

			var a = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.IsNotNull(a);
			Assert.AreEqual(a.Count, 1);
			Assert.AreEqual(a.StackTrace, exception.StackTrace);
			Assert.AreEqual(a.Source, exception.Source);

			//Log the same exception again
			db.Log(exception, null, null);

			Assert.AreEqual(db.ExceptionLogs.Count(), 1);

			var b = db.ExceptionLogs.SingleOrDefault(x => x.Message == exception.Message);
			Assert.AreEqual(b.Count, 2);
			Assert.AreEqual(a.Id, b.Id);

			db.Database.EnsureDeleted();
		}
	}
}