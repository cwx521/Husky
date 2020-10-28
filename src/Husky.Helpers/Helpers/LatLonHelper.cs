using System;
using Husky.Lbs;

namespace Husky
{
	public static class LatLonHelper
	{
		private const double xPI = Math.PI * 3000 / 180;

		public static Distance StraightDistanceTo(this Location one, Location another) => new Distance {
			From = one,
			To = another,
			Mode = DistanceMode.Straight,
			Meters = one.StraightMetersTo(another)
		};

		public static int StraightMetersTo(this Location p1, Location p2) {
			if ( p1.LatLonType != p2.LatLonType ) {
				throw new ArgumentException("This two locations are in different coordinate standards");
			}
			var radLat1 = p1.Lat * Math.PI / 180.0;
			var radLat2 = p2.Lat * Math.PI / 180.0;
			var a = radLat1 - radLat2;
			var b = p1.Lon * Math.PI / 180.0 - p2.Lon * Math.PI / 180.0;
			var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
			return (int)Math.Round(s * 6378.137 * 1000);
		}

		public static Location ConvertToBaiduLatLon(this Location latlon) {
			if ( (int)latlon.LatLonType == (int)LatLonType.Baidu ) {
				latlon.LatLonType = LatLonType.Baidu;
				return latlon;
			}
			var z = Math.Sqrt(latlon.Lon * latlon.Lon + latlon.Lat * latlon.Lat) + 0.00002 * Math.Sin(latlon.Lat * xPI);
			var theta = Math.Atan2(latlon.Lat, latlon.Lon) + 0.000003 * Math.Cos(latlon.Lon * xPI);
			return new Location {
				Lat = z * Math.Sin(theta) + 0.006,
				Lon = z * Math.Cos(theta) + 0.0065,
				LatLonType = LatLonType.Baidu
			};
		}

		public static Location ConvertToTencentLatLon(this Location latlon) {
			if ( (int)latlon.LatLonType == (int)LatLonType.Tencent ) {
				latlon.LatLonType = LatLonType.Tencent;
				return latlon;
			}
			var lat = latlon.Lat - 0.006;
			var lon = latlon.Lon - 0.0065;
			var z = Math.Sqrt(lon * lon + lat * lat) - 0.00002 * Math.Sin(lat * xPI);
			var theta = Math.Atan2(lat, lon) - 0.000003 * Math.Cos(lon * xPI);
			return new Location {
				Lat = z * Math.Sin(theta),
				Lon = z * Math.Cos(theta),
				LatLonType = LatLonType.Tencent
			};
		}
	}
}
