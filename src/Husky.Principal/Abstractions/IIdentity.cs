using System;

namespace Husky.Principal
{
	public interface IIdentity
	{
		int Id { get; set; }
		string DisplayName { get; set; }

		bool IsAuthenticated { get; }
		bool IsAnonymous { get; }
	}
}