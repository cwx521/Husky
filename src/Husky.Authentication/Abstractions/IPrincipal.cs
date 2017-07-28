using System;

namespace Husky.Authentication.Abstractions
{
	public interface IPrincipal
	{
		string IdString { get; }
		string DisplayName { get; }
		bool IsAuthenticated { get; }
		bool IsAnonymous { get; }
		IIdentityManager IdentityManager { get; }
	}
}