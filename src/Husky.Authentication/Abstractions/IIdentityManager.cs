using System;

namespace Husky.Authentication.Abstractions
{
	public interface IIdentityManager<T> where T : IFormattable, IEquatable<T>
	{
		Identity<T> ReadIdentity();
		void SaveIdentity(Identity<T> identity);
		void DeleteIdentity();
	}
}