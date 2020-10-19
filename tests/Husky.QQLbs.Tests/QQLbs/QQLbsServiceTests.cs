﻿using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
					Assert.AreEqual(31, Math.Floor(actual.Location.Value.Lat));
					Assert.AreEqual(120, Math.Floor(actual.Location.Value.Lon));
					Assert.AreEqual(LatLonType.Tencent, actual.Location.Value.LatLonType);
				};

				var ip = "49.73.123.252";
				var address = await qqLbs.GetAddress(IPAddress.Parse(ip));
				assert((Address)address);

				var lonlat = address.Location.Value;
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
				var latlon1 = new Location { Lat = 31.317064f, Lon = 120.680137f, LatLonType = LatLonType.Tencent };
				var latlon2 = new Location { Lat = 31.315506f, Lon = 120.670792f, LatLonType = LatLonType.Tencent };

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
				var latlon1 = new Location { Lat = 31.317064f, Lon = 120.680137f, LatLonType = LatLonType.Tencent };
				var latlon2 = new Location { Lat = 31.315506f, Lon = 120.670792f, LatLonType = LatLonType.Tencent };

				var distances = await qqLbs.GetDistances(latlon1, new Location[] { latlon2 }, DistanceMode.Driving);
				foreach ( var distance in distances ) {
					Assert.IsTrue(Math.Abs(distance.TravelTimeEstimate.TotalMinutes - 4) < 1);
					Assert.IsTrue(Math.Abs(distance.Meters - 1400) < 500);
				}
			}
		}
	}
}