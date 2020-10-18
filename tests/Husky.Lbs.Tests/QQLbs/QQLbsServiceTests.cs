using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.Lbs.QQLbs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs.QQLbs.Tests
{
	[TestClass()]
	public class QQLbsServiceTests
	{
		private readonly string _key = "";

		[TestMethod()]
		public async Task MarshalTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);

				static void assert(Address actual) {
					Assert.IsNotNull(actual);
					Assert.IsTrue(actual.Province.Contains("江苏"));
					Assert.IsTrue(actual.City.Contains("苏州"));
					Assert.AreEqual(31, Math.Floor(actual.LatLon.Value.Lat));
					Assert.AreEqual(120, Math.Floor(actual.LatLon.Value.Lon));
					Assert.AreEqual(LatLonType.Tencent, actual.LatLon.Value.LatLonType);
				};

				var ip = "49.73.123.252";
				var address = await qqLbs.GetAddress(IPAddress.Parse(ip));
				assert((Address)address);

				var lonlat = address.LatLon.Value;
				address = await qqLbs.GetAddress(lonlat);
				assert((Address)address);
			}
		}

		[TestMethod()]
		public async Task GetLatLonTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);

				var givenAddress = "江苏省苏州工业园区苏州中心";
				var latlon = await qqLbs.GetLatLon(givenAddress);
				Assert.AreEqual(31, Math.Floor(latlon.Value.Lat));
				Assert.AreEqual(120, Math.Floor(latlon.Value.Lon));
			}
		}

		[TestMethod()]
		public async Task GetDistanceTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);
				var latlon1 = new LatLon { Lat = 31.317064f, Lon = 120.680137f, LatLonType = LatLonType.Tencent };
				var latlon2 = new LatLon { Lat = 31.315506f, Lon = 120.670792f, LatLonType = LatLonType.Tencent };

				foreach ( DistanceMode i in Enum.GetValues(typeof(DistanceMode)) ) {
					var distance = await qqLbs.GetDistance(latlon1, latlon2, i);

					if ( i == DistanceMode.Driving ) {
						Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 4) < 1);
					}
					Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
				}
			}
		}

		[TestMethod()]
		public async Task GetDistanceTests() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);
				var latlon1 = new LatLon { Lat = 31.317064f, Lon = 120.680137f, LatLonType = LatLonType.Tencent };
				var latlon2 = new LatLon { Lat = 31.315506f, Lon = 120.670792f, LatLonType = LatLonType.Tencent };

				var distances = await qqLbs.GetDistances(latlon1, new LatLon[] { latlon2 }, DistanceMode.Driving);
				foreach ( var distance in distances ) {
					Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 4) < 1);
					Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
				}
			}
		}

		[TestMethod()]
		public async Task ConvertionTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);

				var given = new LatLon {
					Lat = 31.316641f,
					Lon = 120.678459f,
					LatLonType = LatLonType.Tencent
				};
				var converted = await qqLbs.ConvertToBaiduLatLon(given);
				var convertedBack = await qqLbs.ConvertToTencentLatLon(converted.Value);

				Assert.IsTrue(Math.Abs(given.Lat - convertedBack.Value.Lat) < 0.0005);
				Assert.IsTrue(Math.Abs(given.Lon - convertedBack.Value.Lon) < 0.0005);
			}
		}
	}
}