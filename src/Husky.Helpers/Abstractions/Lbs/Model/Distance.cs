using System;

namespace Husky.Lbs
{
	public record Distance
	{
		public Location From { get; init; } = null!;
		public Location To { get; init; } = null!;
		public int Meters { get; init; }
		public DistanceMode Mode { get; init; }
		public TimeSpan TravelTimeEstimate { get; init; }
	}
}