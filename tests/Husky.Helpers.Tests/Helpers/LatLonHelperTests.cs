using System;
using Husky.Lbs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Tests
{
	[TestClass()]
	public class LatLonHelperTests
	{
		[TestMethod()]
		public void ConvertTest() {
			var given = new Location {
				Lat = 31.316641f,
				Lon = 120.678459f,
				LatLonType = LatLonType.Tencent
			};
			var converted = given.ConvertToBaiduLatLon();
			var convertedBack = converted.ConvertToTencentLatLon();
			var convertedAgain = convertedBack.ConvertToBaiduLatLon();

			Assert.IsTrue(Math.Abs(given.Lat - convertedBack.Lat) < 0.0005);
			Assert.IsTrue(Math.Abs(given.Lon - convertedBack.Lon) < 0.0005);

			Assert.IsTrue(Math.Abs(converted.Lat - convertedAgain.Lat) < 0.0005);
			Assert.IsTrue(Math.Abs(converted.Lon - convertedAgain.Lon) < 0.0005);
		}
	}
}