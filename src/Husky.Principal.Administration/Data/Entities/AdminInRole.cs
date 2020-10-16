using System;

namespace Husky.Principal.Administration.Data
{
	public class AdminInRole
	{
		public Guid AdminId { get; set; }

		public int RoleId { get; set; }


		// nav props

		public Admin Admin { get; set; } = null!;
		public AdminRole Role { get; set; } = null!;
	}
}
