using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Husky.CommonModules.Users.Data
{
	public class UserWeChat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int UserId { get; set; }

		[MaxLength(32), Column(TypeName = "varchar(32)")]
		public string? PrivateId { get; set; }

		[MaxLength(32), Column(TypeName = "varchar(32)")]
		public string? MobilePlatformOpenId { get; set; }

		[MaxLength(32), Column(TypeName = "varchar(32)")]
		public string? MiniProgramOpenId { get; set; }

		[MaxLength(32), Column(TypeName = "varchar(32)")]
		public string? UnionId { get; set; }

		[MaxLength(36)]
		public string NickName { get; set; } = null!;

		public Sex Sex { get; set; }

		[MaxLength(500), Column(TypeName = "varchar(500)")]
		public string HeadImageUrl { get; set; } = null!;

		[MaxLength(24)]
		public string? Province { get; set; }

		[MaxLength(24)]
		public string? City { get; set; }

		[MaxLength(24)]
		public string? Country { get; set; }

		[MaxLength(128), Column(TypeName = "varchar(128)")]
		public string? AccessToken { get; set; }

		[MaxLength(128), Column(TypeName = "varchar(128)")]
		public string? RefreshToken { get; set; }

		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		[JsonIgnore]
		public User User { get; set; } = null!;
	}
}
