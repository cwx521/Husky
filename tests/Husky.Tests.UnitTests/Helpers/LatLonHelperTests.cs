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
				Lat = 31.316641m,
				Lon = 120.678459m,
				LatLonType = LatLonType.Gcj02
			};
			var converted = given.ConvertToBd09();
			var convertedBack = converted.ConvertToGcj02();
			var convertedAgain = convertedBack.ConvertToBd09();

			Assert.IsTrue(Math.Abs(given.Lat - convertedBack.Lat) < 0.0005m);
			Assert.IsTrue(Math.Abs(given.Lon - convertedBack.Lon) < 0.0005m);

			Assert.IsTrue(Math.Abs(converted.Lat - convertedAgain.Lat) < 0.0005m);
			Assert.IsTrue(Math.Abs(converted.Lon - convertedAgain.Lon) < 0.0005m);
		}
	}
}