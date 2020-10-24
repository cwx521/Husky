using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky.Lbs;

namespace Husky.Principal.Users.Data
{
	public class UserAddress : IAddress
	{
		[Key]
		public int Id { get; set; }

		[NeverUpdate]
		public int UserId { get; set; }

		public Location? Location { get; set; }

		[StringLength(100)]
		public string? DisplayAddress { get; set; }

		[StringLength(100)]
		public string? DisplayAddressAlternate { get; set; }

		[StringLength(24), Required]
		public string? Province { get; set; } = null!;

		[StringLength(24), Required]
		public string? City { get; set; } = null!;

		[StringLength(24)]
		public string? District { get; set; }

		[StringLength(50)]
		public string? Street { get; set; }

		[StringLength(50)]
		public string? AccuratePlace { get; set; }

		[StringLength(16)]
		public string? ContactName { get; set; }

		[StringLength(11), Column(TypeName = "varchar(11)"), Phone]
		public string? ContactPhoneNumber { get; set; }

		public bool IsDefault { get; set; }

		public RowStatus Status { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;


		// calculation

		public override string ToString() => DisplayAddressAlternate
			?? DisplayAddress
			?? Province + City + District + (AccuratePlace ?? Street);
	}
}
