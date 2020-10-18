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
}
