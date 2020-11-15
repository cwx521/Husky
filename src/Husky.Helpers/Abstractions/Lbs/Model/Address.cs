namespace Husky.Lbs
{
	public record Address
	{
		public Location? Location { get; init; }

		public string? DisplayAddress { get; init; }
		public string? DisplayAddressAlternate { get; init; }

		public string? Province { get; init; }
		public string? City { get; init; }
		public string? District { get; init; }
		public string? Street { get; init; }
		public string? AccuratePlace { get; init; }

		public override string ToString() => DisplayAddressAlternate
			?? DisplayAddress
			?? Province + City + District + (AccuratePlace ?? Street);
	}
}
