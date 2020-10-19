using System;

namespace Husky.Lbs
{
	public interface IDistance
	{
		Location From { get; set; }
		Location To { get; set; }
		int Meters { get; set; }
		TimeSpan TravelTimeEstimate { get; set; }
		DistanceMode Mode { get; set; }
	}
}
