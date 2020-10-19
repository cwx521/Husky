using System;

namespace Husky.Lbs
{
	public class Distance : IDistance
	{
		public LatLon From { get; set; }
		public LatLon To { get; set; }
		public int Meters { get; set; }
		public TimeSpan TravelTimeEstimate { get; set; }
		public DistanceMode Mode { get; set; }
	}

	public static class DistanceHelper
	{
		public static IDistance StraightDistanceTo(this LatLon one, LatLon another) => new Distance {
			From = one,
			To = another,
			Mode = DistanceMode.Straight,
			Meters = one.StraightMetersTo(another)
		};
	}
}
