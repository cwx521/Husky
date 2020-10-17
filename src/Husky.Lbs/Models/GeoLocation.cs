namespace Husky.Lbs
{
	public class GeoLocation
	{
		public decimal? Lat { get; set; }
		public decimal? Lon { get; set; }
		public string? Ip { get; set; } = null!;

		public string? Nation { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? District { get; set; }
		public string? Street { get; set; }
		public string? StreetNumber { get; set; }
		public string? Address { get; set; }
		public string? AddressAlternate { get; set; }
	}
}
