using System;
using System.Net;
using Husky.Lbs.QQLbs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Lbs.Tests
{
	[TestClass()]
	public class QQLbsTests
	{
		[TestMethod()]
		public void QueryTest() {
			var key = "";

			if ( !string.IsNullOrEmpty(key) ) {
				var ip = "49.73.123.252";

				var qqLbs = new QQLbsService(key);
				var location = qqLbs.QueryFromIp(IPAddress.Parse(ip)).Result;

				Assert.IsNotNull(location);
				Assert.IsTrue(location.Nation.Contains("中国"));
				Assert.IsTrue(location.Province.Contains("江苏"));
				Assert.IsTrue(location.City.Contains("苏州"));
				Assert.AreEqual(Math.Floor(location.Lat ?? 0), 31);
				Assert.AreEqual(Math.Floor(location.Lon ?? 0), 120);
			}
		}
	}
}