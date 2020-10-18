using System;
using Husky.Lbs;

namespace Husky
{
	public static class LatLonHelper
	{
		public static int StraightMetersTo(this LatLon p1, LatLon p2) {
			if ( p1.LatLonType != p2.LatLonType ) {
				throw new ArgumentException("坐标系不一致");
			}
			var radLat1 = p1.Lat * Math.PI / 180.0;
			var radLat2 = p2.Lat * Math.PI / 180.0;
			var a = radLat1 - radLat2;
			var b = p1.Lon * Math.PI / 180.0 - p2.Lon * Math.PI / 180.0;
			var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
			return (int)Math.Round(s * 6378.137 * 1000);
		}

		public static LatLon ConvertToBaiduLatLon(this LatLon latlon) {
			if ( (int)latlon.LatLonType == (int)LatLonType.Baidu ) {
				latlon.LatLonType = LatLonType.Baidu;
				return latlon;
			}
			var xPI = Math.PI * 3000 / 180;
			var z = Math.Sqrt(latlon.Lon * latlon.Lon + latlon.Lat * latlon.Lat) + 0.00002 * Math.Sin(latlon.Lat * xPI);
			var theta = Math.Atan2(latlon.Lat, latlon.Lon) + 0.000003 * Math.Cos(latlon.Lon * xPI);
			return new LatLon {
				Lat = (float)(z * Math.Sin(theta) + 0.006),
				Lon = (float)(z * Math.Cos(theta) + 0.0065),
				LatLonType = LatLonType.Baidu
			};
		}
	}
}
