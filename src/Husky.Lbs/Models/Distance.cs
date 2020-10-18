using System;

namespace Husky.Lbs
{
	public class Distance : IDistance
	{
		public ILatLon From { get; set; } = null!;
		public ILatLon To { get; set; } = null!;
		public int Meters { get; set; }
		public TimeSpan TravelTimeEstimate { get; set; }
		public DistanceMode Mode { get; set; }
	}

	public static class DistanceHelper
	{
		public static IDistance StraightDistanceTo(this ILatLon one, ILatLon another) => new Distance {
			From = one,
			To = another,
			Mode = DistanceMode.Straight,
			Meters = one.StraightMetersTo(another)
		};
	}
}
