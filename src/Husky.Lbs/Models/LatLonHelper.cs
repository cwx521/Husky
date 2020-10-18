using System;

namespace Husky.Lbs
{
	public static class LatLonHelper
	{
		public static IDistance StraightDistanceTo(this ILatLon one, ILatLon another) => new Distance {
			From = one,
			To = another,
			Mode = DistanceMode.Straight,
			Meters = one.StraightMetersTo(another)
		};

		public static int StraightMetersTo(this ILatLon p1, ILatLon p2) {
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
	}
}
