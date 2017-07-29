using System;

namespace Husky.Authentication.Abstractions
{
	public interface IIdentity
	{
		string IdString { get; set; }
		string DisplayName { get; set; }

		bool IsAuthenticated { get; }
		bool IsAnonymous { get; }

		T? Id<T>() where T : struct, IFormattable, IEquatable<T>;
	}
}