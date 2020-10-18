using System;

namespace Husky.Lbs
{
	public interface IDistance
	{
		ILatLon From { get; set; }
		ILatLon To { get; set; }
		int Meters { get; set; }
		TimeSpan TravelTimeEstimate { get; set; }
		DistanceMode Mode { get; set; }
	}
}
