using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Husky.Users.Data
{
	public class UserAddress
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }

		[Column(TypeName = "decimal(11, 6)")]
		public decimal? Lon { get; set; }

		[Column(TypeName = "decimal(11, 6)")]
		public decimal? Lat { get; set; }

		[MaxLength(16)]
		public string Province { get; set; } = null!;

		[MaxLength(16)]
		public string City { get; set; } = null!;

		[MaxLength(16)]
		public string? District { get; set; }

		[MaxLength(100)]
		public string? Location { get; set; }

		[MaxLength(16)]
		public string? ContactName { get; set; }

		[MaxLength(11), Phone, Column(TypeName = "varchar(11)")]
		public string? ContactPhoneNumber { get; set; }

		public bool IsDefault { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// calculation

		public string FullAddress => Province + City + District + Location;
		public string FullAddressSplitBySpace => string.Join(" ", Province, City, District, Location);


		// nav props

		[JsonIgnore]
		public User User { get; set; } = null!;
	}
}
