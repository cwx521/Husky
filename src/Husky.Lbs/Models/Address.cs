namespace Husky.Lbs
{
	public class Address : IAddress
	{
		public float Lat { get; set; }
		public float Lon { get; set; }
		public LatLonType LatLonType { get; set; } = LatLonType.Tencent;

		public string? DisplayAddress { get; set; }
		public string? DisplayAddressAlternate { get; set; }

		public string? Nation { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? District { get; set; }
		public string? Street { get; set; }
		public string? StreetNumber { get; set; }
		public int? ZipCode { get; set; }
	}
}
