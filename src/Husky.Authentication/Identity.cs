using Husky.Sugar;

namespace Husky.Authentication
{
	public class Identity
	{
		public string IdString { get; set; }
		public string DisplayName { get; set; }

		public virtual bool IsAnonymous => !string.IsNullOrWhiteSpace(IdString);
		public virtual bool IsAuthenticated => !IsAnonymous;

		public T Id<T>() => IdString.As<T>();
	}
}