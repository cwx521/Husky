using System;

namespace Husky.Lbs
{
	public class Distance : IDistance
	{
		public Location From { get; set; }
		public Location To { get; set; }
		public int Meters { get; set; }
		public TimeSpan TravelTimeEstimate { get; set; }
		public DistanceMode Mode { get; set; }
	}

	public static class DistanceHelper
	{
		public static IDistance StraightDistanceTo(this Location one, Location another) => new Distance {
			From = one,
			To = another,
			Mode = DistanceMode.Straight,
			Meters = one.StraightMetersTo(another)
		};
	}
}
