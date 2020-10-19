using System;

namespace Husky.Lbs
{
	public struct Location
	{
		public Location(float latitude, float longitude) {
			Lat = latitude;
			Lon = longitude;
			LatLonType = LatLonType.Tencent;
		}

		public Location(float latitude, float longitude, LatLonType type) {
			Lat = latitude;
			Lon = longitude;
			LatLonType = type;
		}

		public float Lat { get; set; }
		public float Lon { get; set; }
		public LatLonType LatLonType { get; set; }

		public override string ToString() => $"{Lat},{Lon}";

		public static Location Parse(string latlon) {
			if ( latlon == null ) {
				throw new ArgumentNullException(nameof(latlon));
			}
			var array = latlon.Split<float>(',');
			if ( array.Length != 2 || (array[0] == 0 && array[1] == 0) ) {
				throw new FormatException();
			}
			return new Location(array[0], array[1]);
		}

		public static bool TryParse(string latlon, out Location location) {
			if ( latlon != null ) {
				var array = latlon.Split<float>(',');

				if ( array.Length == 2 && (array[0] != 0 || array[1] != 0) ) {
					location = new Location(array[0], array[1]);
					return true;
				}
			}
			location = new Location();
			return false;
		}
	}
}
