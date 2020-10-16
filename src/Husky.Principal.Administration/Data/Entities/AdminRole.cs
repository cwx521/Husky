using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Husky.Principal.Administration.Data
{
	public class AdminRole
	{
		[Key]
		public int Id { get; set; }

		[StringLength(24), Required, Unique]
		public string RoleName { get; set; } = null!;

		public long Powers { get; set; }


		// nav props

		public List<AdminInRole> GrantedToAdmins { get; set; } = new List<AdminInRole>();
	}
}
