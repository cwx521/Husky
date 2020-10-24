namespace Husky.Lbs
{
	public interface IAddress
	{
		Location? Location { get; set; }

		string? DisplayAddress { get; set; }
		string? DisplayAddressAlternate { get; set; }

		string? Province { get; set; }
		string? City { get; set; }
		string? District { get; set; }
		string? Street { get; set; }
		string? AccuratePlace { get; set; }
	}
}
