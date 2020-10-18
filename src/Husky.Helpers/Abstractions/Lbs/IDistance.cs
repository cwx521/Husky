using System;

namespace Husky.Lbs
{
	public interface IDistance
	{
		LatLon From { get; set; }
		LatLon To { get; set; }
		int Meters { get; set; }
		TimeSpan TravelTimeEstimate { get; set; }
		DistanceMode Mode { get; set; }
	}
}
