using Microsoft.VisualStudio.TestTools.UnitTesting;
using Husky.Lbs.QQLbs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Husky.Lbs.QQLbs.Tests
{
	[TestClass()]
	public class QQLbsServiceTests
	{
		private string _key = "";

		[TestMethod()]
		public void MarshalTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);

				static void assert(IAddress actual) {
					Assert.IsNotNull(actual);
					Assert.IsTrue(actual.Nation.Contains("中国"));
					Assert.IsTrue(actual.Province.Contains("江苏"));
					Assert.IsTrue(actual.City.Contains("苏州"));
					Assert.AreEqual(31, Math.Floor(actual.Lat));
					Assert.AreEqual(120, Math.Floor(actual.Lon));
					Assert.AreEqual(LatLonType.Tencent, actual.LatLonType);
				};

				var ip = "49.73.123.252";
				var address = qqLbs.GetAddress(IPAddress.Parse(ip)).Result;
				assert(address);

				var lonlat = (ILatLon)address!;
				address = qqLbs.GetAddress(lonlat).Result;
				assert(address);
			}
		}

		[TestMethod()]
		public void GetLatLonTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);

				var givenAddress = "江苏省苏州工业园区苏州中心";
				var latlon = qqLbs.GetLatLon(givenAddress).Result;
				Assert.AreEqual(31, Math.Floor(latlon.Lat));
				Assert.AreEqual(120, Math.Floor(latlon.Lon));
			}
		}

		[TestMethod()]
		public void GetDistanceTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);
				var latlon1 = new LatLon { Lat = 31.317064f, Lon = 120.680137f, LatLonType = LatLonType.Tencent };
				var latlon2 = new LatLon { Lat = 31.315506f, Lon = 120.670792f, LatLonType = LatLonType.Tencent };

				foreach ( DistanceMode i in Enum.GetValues(typeof(DistanceMode)) ) {
					var distance = qqLbs.GetDistance(latlon1, latlon2, i).Result;

					if ( i == DistanceMode.Driving ) {
						Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 4) < 1);
					}
					Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
				}
			}
		}

		[TestMethod()]
		public void GetDistanceTests() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);
				var latlon1 = new LatLon { Lat = 31.317064f, Lon = 120.680137f, LatLonType = LatLonType.Tencent };
				var latlon2 = new LatLon { Lat = 31.315506f, Lon = 120.670792f, LatLonType = LatLonType.Tencent };

				var distances = qqLbs.GetDistances(latlon1, new ILatLon[] { latlon2 }, DistanceMode.Driving).Result;
				foreach ( var distance in distances ) {
					Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 4) < 1);
					Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
				}
			}
		}

		[TestMethod()]
		public void ConvertionTest() {
			if ( !string.IsNullOrEmpty(_key) ) {
				var qqLbs = new QQLbsService(_key);

				var given = new LatLon {
					Lat = 31.316641f,
					Lon = 120.678459f,
					LatLonType = LatLonType.Tencent
				};
				var converted = qqLbs.ConvertToBaiduLatLon(given).Result;
				var convertedBack = qqLbs.ConvertToTencentLatLon(converted).Result;

				Assert.IsTrue(Math.Abs(given.Lat - convertedBack.Lat) < 0.0005);
				Assert.IsTrue(Math.Abs(given.Lon - convertedBack.Lon) < 0.0005);
			}
		}
	}
}