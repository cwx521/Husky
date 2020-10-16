using System;

namespace Husky.Principal.Administration
{
	public class AdminViewModel
	{
		public Guid? Id { get; internal set; }
		public string? DisplayName { get; internal set; }
		public string[]? Roles { get; internal set; }
		public long? Powers { get; internal set; }
	}
}