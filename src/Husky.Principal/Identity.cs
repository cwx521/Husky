using System;
using System.Text.Json;

namespace Husky.Principal
{
	public class Identity : IIdentity
	{
		public virtual Guid AnonymousId { get; set; } = Guid.NewGuid();

		public virtual int Id { get; set; }
		public virtual string DisplayName { get; set; } = null!;
		public virtual bool IsConsolidated { get; set; }

		public virtual bool IsAnonymous => Id == 0;
		public virtual bool IsAuthenticated => !IsAnonymous;

		public override string ToString() => $"{Id}|{DisplayName}|{IsConsolidated}|{AnonymousId}";
	}
}