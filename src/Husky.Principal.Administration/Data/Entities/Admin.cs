using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Husky.Principal.Administration.Data
{
	public class Admin
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Unique]
		public int UserId { get; set; }

		[StringLength(36), Required]
		public string DisplayName { get; set; } = null!;

		public bool IsInitiator { get; set; }

		public RowStatus Status { get; set; }

		[DefaultValueSql("getdate()"), NeverUpdate]
		public DateTime CreatedTime { get; set; } = DateTime.Now;


		// nav props

		public List<AdminInRole> InRoles { get; set; } = new List<AdminInRole>();
	}
}
