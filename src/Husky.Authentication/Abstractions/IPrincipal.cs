using System;

namespace Husky.Authentication.Abstractions
{
	public interface IPrincipal<T> where T : IFormattable, IEquatable<T>
	{
		T Id { get; }
		string DisplayName { get; }
		bool IsAuthenticated { get; }
		bool IsAnonymous { get; }
		IIdentityManager<T> IdentityManager { get; }
	}
}