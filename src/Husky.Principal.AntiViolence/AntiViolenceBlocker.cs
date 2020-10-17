using System;

namespace Husky.Principal.AntiViolence
{
	public sealed partial class AntiViolenceBlocker
	{
		public AntiViolenceBlocker(IPrincipalUser principal) {
			_me = principal;
		}

		private readonly IPrincipalUser _me;

		internal DateTime GetTimer() {
			return (DateTime)_me.CacheData().GetOrAdd(nameof(AntiViolenceBlocker), key => DateTime.Now.AddDays(-1));
		}

		internal void SetTimer(DateTime? time = null) {
			var val = time ?? DateTime.Now;
			_me.CacheData()?.AddOrUpdate(nameof(AntiViolenceBlocker), val, (key, old) => val);
		}

		public void ClearTimer() {
			SetTimer(DateTime.MinValue);
		}
	}
}
