using System;

namespace Husky.Authentication
{
	public class Identity<T> where T : IFormattable, IEquatable<T>
	{
		public T Id { get; set; }
		public string DisplayName { get; set; }

		public virtual bool IsAnonymous => Id == null || Id.Equals(default(T));
		public virtual bool IsAuthenticated => !IsAnonymous;
	}
}