using System;
using Husky.Lbs;

namespace Husky
{
	public static class LatLonHelper
	{
		private const double xPI = Math.PI * 3000 / 180;

		public static Distance StraightDistanceTo(this Location one, Location another) => new() {
			From = one,
			To = another,
			Mode = DistanceMode.Straight,
			Meters = one.StraightMetersTo(another)
		};

		public static int StraightMetersTo(this Location one, Location another) {
			if (one.LatLonType != another.LatLonType) {
				throw new ArgumentException("These two locations are in different coordinate standards");
			}
			var radLat1 = (double)one.Lat * Math.PI / 180.0;
			var radLat2 = (double)another.Lat * Math.PI / 180.0;
			var a = radLat1 - radLat2;
			var b = (double)one.Lon * Math.PI / 180.0 - (double)another.Lon * Math.PI / 180.0;
			var s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
			return (int)Math.Round(s * 6378.137 * 1000);
		}

		public static Location ConvertToBd09(this Location latlon) {
			if ((int)latlon.LatLonType == (int)LatLonType.Bd09) {
				latlon.LatLonType = LatLonType.Bd09;
				return latlon;
			}
			var z = Math.Sqrt((double)(latlon.Lon * latlon.Lon + latlon.Lat * latlon.Lat)) + 0.00002 * Math.Sin((double)latlon.Lat * xPI);
			var theta = Math.Atan2((double)latlon.Lat, (double)latlon.Lon) + 0.000003 * Math.Cos((double)latlon.Lon * xPI);
			return new Location {
				Lat = (decimal)(z * Math.Sin(theta) + 0.006),
				Lon = (decimal)(z * Math.Cos(theta) + 0.0065),
				LatLonType = LatLonType.Bd09
			};
		}

		public static Location ConvertToGcj02(this Location latlon) {
			if ((int)latlon.LatLonType == (int)LatLonType.Gcj02) {
				latlon.LatLonType = LatLonType.Gcj02;
				return latlon;
			}
			var lat = (double)latlon.Lat - 0.006;
			var lon = (double)latlon.Lon - 0.0065;
			var z = Math.Sqrt(lon * lon + lat * lat) - 0.00002 * Math.Sin(lat * xPI);
			var theta = Math.Atan2(lat, lon) - 0.000003 * Math.Cos(lon * xPI);
			return new Location {
				Lat = (decimal)(z * Math.Sin(theta)),
				Lon = (decimal)(z * Math.Cos(theta)),
				LatLonType = LatLonType.Gcj02
			};
		}
	}
}
