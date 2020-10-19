﻿namespace Husky.Lbs
{
	public class Address : IAddress
	{
		public Location? Location { get; set; }

		public string? DisplayAddress { get; set; }
		public string? DisplayAddressAlternate { get; set; }

		public string? Province { get; set; }
		public string? City { get; set; }
		public string? District { get; set; }
		public string? Street { get; set; }
		public string? StreetNumber { get; set; }

		public override string ToString() => DisplayAddressAlternate
			?? DisplayAddress
			?? Province + City + District + (StreetNumber ?? Street);
	}
}