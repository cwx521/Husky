using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Lbs.QQLbs.Tests
{
	[TestClass()]
	public class QQLbsServiceTests
	{
		//attention: fill the required values to run this test

		private readonly string _key = "";

		[TestMethod()]
		public async Task MarshalTest() {
			if ( string.IsNullOrEmpty(_key) ) {
				return;
			}

			var qqLbs = new QQLbsService(_key);

			static void assert(Address actual) {
				Assert.IsNotNull(actual);
				Assert.IsTrue(actual.Province.Contains("江苏"));
				Assert.IsTrue(actual.City.Contains("苏州"));
				Assert.AreEqual(31, Math.Floor(actual.Location.Lat));
				Assert.AreEqual(120, Math.Floor(actual.Location.Lon));
				Assert.AreEqual(LatLonType.Gcj02, actual.Location.LatLonType);
			};

			var ip = "49.73.123.252";
			var address = await qqLbs.GetAddressAsync(IPAddress.Parse(ip));
			assert((Address)address);

			var lonlat = address.Location;
			address = await qqLbs.GetAddressAsync(lonlat);
			assert((Address)address);
		}

		[TestMethod()]
		public async Task GetLatLonTest() {
			if ( string.IsNullOrEmpty(_key) ) {
				return;
			}

			var qqLbs = new QQLbsService(_key);
			var givenAddress = "江苏省苏州工业园区苏州中心";
			var latlon = await qqLbs.GetLatLonAsync(givenAddress);

			Assert.AreEqual(31, Math.Floor(latlon.Lat));
			Assert.AreEqual(120, Math.Floor(latlon.Lon));
		}

		[TestMethod()]
		public async Task GetDistanceTest() {
			if ( string.IsNullOrEmpty(_key) ) {
				return;
			}

			var qqLbs = new QQLbsService(_key);
			var latlon1 = new Location { Lat = 31.317064m, Lon = 120.680137m, LatLonType = LatLonType.Gcj02 };
			var latlon2 = new Location { Lat = 31.315506m, Lon = 120.670792m, LatLonType = LatLonType.Gcj02 };

			foreach ( DistanceMode i in Enum.GetValues(typeof(DistanceMode)) ) {
				var distance = await qqLbs.GetDistanceAsync(latlon1, latlon2, i);

				if ( i == DistanceMode.Driving ) {
					Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 5) < 2);
				}
				Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
			}
		}

		[TestMethod()]
		public async Task GetDistancesTest() {
			if ( string.IsNullOrEmpty(_key) ) {
				return;
			}

			var qqLbs = new QQLbsService(_key);
			var latlon1 = new Location { Lat = 31.317064m, Lon = 120.680137m, LatLonType = LatLonType.Gcj02 };
			var latlon2 = new Location { Lat = 31.315506m, Lon = 120.670792m, LatLonType = LatLonType.Gcj02 };

			var distances = await qqLbs.GetDistancesAsync(latlon1, new Location[] { latlon2 }, DistanceMode.Driving);
			foreach ( var distance in distances ) {
				Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 5) < 2);
				Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
			}
		}
	}
}