namespace Husky.Lbs
{
	public interface IAddress
	{
		LatLon? LatLon { get; set; }

		string? DisplayAddress { get; set; }
		string? DisplayAddressAlternate { get; set; }

		string? Province { get; set; }
		string? City { get; set; }
		string? District { get; set; }
		string? Street { get; set; }
		string? StreetNumber { get; set; }
	}
}
