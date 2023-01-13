using System;

namespace Husky.Principal.AntiViolence
{
	public sealed partial class AntiViolenceDefender
	{
		public AntiViolenceDefender(IPrincipalUser principal) {
			_me = principal;
		}

		private readonly IPrincipalUser _me;

		internal DateTime GetTimer() {
			return (DateTime)_me.Cache().GetOrAdd(nameof(AntiViolenceDefender), _ => DateTime.MinValue);
		}

		internal void SetTimer(DateTime? time = null) {
			var val = time ?? DateTime.Now;
			_me.Cache().AddOrUpdate(nameof(AntiViolenceDefender), val, (_, _) => val);
		}

		public void ClearTimer() {
			SetTimer(DateTime.MinValue);
		}
	}
}
