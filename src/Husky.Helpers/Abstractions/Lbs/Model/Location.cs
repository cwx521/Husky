using System;

namespace Husky.Lbs
{
	public class Location
	{
		public Location() {
		}

		public Location(double latitude, double longitude) {
			Lat = latitude;
			Lon = longitude;
			LatLonType = LatLonType.Gcj02;
		}

		public Location(double latitude, double longitude, LatLonType type) {
			Lat = latitude;
			Lon = longitude;
			LatLonType = type;
		}

		public double Lat { get; set; }
		public double Lon { get; set; }
		public LatLonType LatLonType { get; set; }

		public override string ToString() => $"{Lat},{Lon}";

		public static Location Parse(string latlon) {
			if ( latlon == null ) {
				throw new ArgumentNullException(nameof(latlon));
			}
			var array = latlon.Split<double>(',');
			if ( array.Length != 2 || (array[0] == 0 && array[1] == 0) ) {
				throw new FormatException();
			}
			return new Location(array[0], array[1]);
		}

		public static bool TryParse(string latlon, out Location location) {
			if ( latlon != null ) {
				var array = latlon.Split<double>(',');

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
