namespace Husky.Lbs
{
	public class GeoLocation
	{
		public string Ip { get; set; } = null!;
		public decimal? Lat { get; set; }
		public decimal? Lon { get; set; }
		public string? Nation { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? District { get; set; }
	}
}
