using System;
using Husky.Authentication.Abstractions;
using Husky.Sugar;

namespace Husky.Authentication
{
	public class Identity : IIdentity
	{
		public virtual string IdString { get; set; }
		public virtual string DisplayName { get; set; }

		public virtual bool IsAnonymous => !string.IsNullOrWhiteSpace(IdString);
		public virtual bool IsAuthenticated => !IsAnonymous;

		public T? Id<T>() where T : struct, IFormattable, IEquatable<T> => IdString.As<T>();
	}
}