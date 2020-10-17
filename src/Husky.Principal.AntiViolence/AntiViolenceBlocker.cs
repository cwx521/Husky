using System;

namespace Husky.Principal.AntiViolence
{
	public sealed partial class AntiViolenceBlocker
	{
		internal AntiViolenceBlocker(IPrincipalUser principal) {
			_me = principal;
		}

		private readonly IPrincipalUser _me;

		internal DateTime GetTimer() {
			return (DateTime)_me.SessionData().GetOrAdd(nameof(AntiViolenceBlocker), key => DateTime.Now.AddDays(-1));
		}

		internal void SetTimer(DateTime? time = null) {
			var val = time ?? DateTime.Now;
			_me.SessionData()?.AddOrUpdate(nameof(AntiViolenceBlocker), val, (key, old) => val);
		}

		internal void ClearTimer() {
			SetTimer(DateTime.MinValue);
		}
	}
}
