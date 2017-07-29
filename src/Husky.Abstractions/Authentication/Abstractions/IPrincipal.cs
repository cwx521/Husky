using System;

namespace Husky.Authentication.Abstractions
{
	public interface IPrincipal : IIdentity
	{
		IIdentityManager IdentityManager { get; }
		IServiceProvider ServiceProvider { get; }
	}
}