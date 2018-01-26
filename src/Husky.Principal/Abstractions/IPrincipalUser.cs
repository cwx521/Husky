using System;

namespace Husky.Principal
{
	public interface IPrincipalUser : IIdentity
	{
		IIdentityManager IdentityManager { get; }
		IServiceProvider ServiceProvider { get; }
	}
}