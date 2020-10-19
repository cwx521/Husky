using System;

namespace Husky.Lbs
{
	public class Distance
	{
		public Location From { get; set; }
		public Location To { get; set; }
		public int Meters { get; set; }
		public DistanceMode Mode { get; set; }
		public TimeSpan TravelTimeEstimate { get; set; }
	}
}
