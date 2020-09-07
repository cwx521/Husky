using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Husky;

namespace Husky.CommonModules.Users.Data
{
	public class UserMessage
	{
		[Key]
		public int Id { get; set; }

		public int UserId { get; set; }

		public int? CommonContentId { get; set; }

		[MaxLength(4000)]
		public string Content { get; set; } = null!;

		public bool IsRead { get; set; }

		public RowStatus State { get; set; } = RowStatus.Active;

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public User User { get; set; } = null!;
		public UserMessageCommonContent? CommonContent { get; set; }
	}
}
