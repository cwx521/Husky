using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Husky.CommonModules.Users.Data
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
		public string District { get; set; } = null!;

		[MaxLength(100)]
		public string? DetailAddress { get; set; }

		[MaxLength(16)]
		public string? ContactName { get; set; }

		[MaxLength(11), Phone]
		public string? ContactPhoneNumber { get; set; }

		public bool IsDefault { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// calculation

		public string FullAddress => Province + City + District + DetailAddress;
		public string FullAddressSplitBySpace => string.Join(" ", Province, City, District, DetailAddress);


		// nav props

		[JsonIgnore]
		public User User { get; set; } = null!;
	}
}
