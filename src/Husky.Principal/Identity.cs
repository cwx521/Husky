using System;

namespace Husky.Principal
{
	public class Identity : IIdentity
	{
		public virtual string IdString { get; set; }
		public virtual string DisplayName { get; set; }

		public virtual bool IsAnonymous => string.IsNullOrWhiteSpace(IdString);
		public virtual bool IsAuthenticated => !IsAnonymous;

		public virtual T Id<T>() where T : struct, IFormattable, IEquatable<T> {
			if ( IsAnonymous ) {
				return default;
			}
			return IdString.As<T>();
		}
	}
}