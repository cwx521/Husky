using System;

namespace Husky.Principal
{
	public interface IPrincipalUser : IIdentity, IIdentityAnonymous
	{
		IServiceProvider ServiceProvider { get; }
		IIdentityManager? IdentityManager { get; }
	}
}