using System;

namespace Husky.Lbs
{
	public struct LatLon : ILatLon
	{
		public LatLon(float latitude, float longitude) {
			Lat = latitude;
			Lon = longitude;
			LatLonType = LatLonType.Tencent;
		}

		public LatLon(float latitude, float longitude, LatLonType type) {
			Lat = latitude;
			Lon = longitude;
			LatLonType = type;
		}

		public float Lat { get; set; }
		public float Lon { get; set; }
		public LatLonType LatLonType { get; set; }

		public override string ToString() => $"{Lat},{Lon}";
	}
}
