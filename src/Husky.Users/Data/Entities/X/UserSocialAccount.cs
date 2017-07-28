//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using Husky.Data.ModelBuilding.Annotations;

//namespace Husky.Users.Data.Entities
//{
//	public class UserSocialAccount
//	{
//		[Key]
//		public Guid UserId { get; set; }

//		[MaxLength(40), Column(TypeName = "varchar(40)")]
//		public string WeChat { get; set; }

//		[MaxLength(40), Column(TypeName = "varchar(40)")]
//		public string Weibo { get; set; }

//		[MaxLength(40), Column(TypeName = "varchar(40)")]
//		public string Facebook { get; set; }

//		[MaxLength(40), Column(TypeName = "varchar(40)")]
//		public string GoogleAccount { get; set; }

//		[MaxLength(40), Column(TypeName = "varchar(40)")]
//		public string Skype { get; set; }

//		[MaxLength(40), Column(TypeName = "varchar(40)")]
//		public string Twitter { get; set; }

//		[Index(IsClustered = true, IsUnique = false)]
//		public DateTime CreatedTime { get; set; } = DateTime.Now;


//		public User User { get; set; }
//	}
//}

