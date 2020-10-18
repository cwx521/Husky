namespace Husky.Lbs
{
	public interface IAddress : ILatLon
	{
		string? DisplayAddress { get; set; }
		string? DisplayAddressAlternate { get; set; }

		string? Nation { get; set; }
		string? Province { get; set; }
		string? City { get; set; }
		string? District { get; set; }
		string? Street { get; set; }
		string? StreetNumber { get; set; }
		int? ZipCode { get; set; }
	}
}
