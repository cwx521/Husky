using System;
using Husky.AspNetCore;

namespace Husky.AspNetCore.Principal
{
	public class Identity : IIdentity
	{
		public virtual string IdString { get; set; }
		public virtual string DisplayName { get; set; }

		public virtual bool IsAnonymous => string.IsNullOrWhiteSpace(IdString);
		public virtual bool IsAuthenticated => !IsAnonymous;

		public virtual T? Id<T>() where T : struct, IFormattable, IEquatable<T> {
			if ( IsAnonymous ) {
				return null;
			}
			var id = IdString.As<T>();
			if ( id.Equals(default(T)) ) {
				return null;
			}
			return id;
		}
	}
}