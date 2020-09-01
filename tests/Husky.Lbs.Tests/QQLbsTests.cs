using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.Lbs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Husky.Lbs.Tests
{
	[TestClass()]
	public class QQLbsTests
	{
		[TestMethod()]
		public void QueryTest() {
			var key = "2S6BZ-APVWD-GCG4F-HLKUO-GQ5VT-EDB3Q";
			var ip = "49.73.123.252";

			var qqLbs = new QQLbs(key);
			var location = qqLbs.Query(IPAddress.Parse(ip)).Result;

			Assert.IsNotNull(location);
			Assert.IsTrue(location.Nation.Contains("中国"));
			Assert.IsTrue(location.Province.Contains("江苏"));
			Assert.IsTrue(location.City.Contains("苏州"));
			Assert.AreEqual(Math.Floor(location.Lat ?? 0), 31);
			Assert.AreEqual(Math.Floor(location.Lon ?? 0), 120);
		}
	}
}