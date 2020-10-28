using System;

namespace Husky.Lbs
{
	public class Distance
	{
		public Location From { get; set; } = null!;
		public Location To { get; set; } = null!;
		public int Meters { get; set; }
		public DistanceMode Mode { get; set; }
		public TimeSpan TravelTimeEstimate { get; set; }
	}
}
