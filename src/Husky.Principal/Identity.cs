using System;

namespace Husky.Principal
{
	public class Identity : IIdentity
	{
		public virtual int Id { get; set; }
		public virtual string DisplayName { get; set; }

		public virtual bool IsAnonymous => Id == 0;
		public virtual bool IsAuthenticated => !IsAnonymous;
	}
}