using System;
using Husky.Principal.SessionData;

namespace Husky.Principal.AntiViolence
{
	public sealed partial class AntiViolenceBlocker
	{
		internal AntiViolenceBlocker(IPrincipalUser principal) {
			_me = principal;
		}

		private readonly IPrincipalUser _me;

		public const string ViewName = "_AntiViolence";

		internal DateTime GetTimer() {
			return _me.Id != 0 && _me.SessionData() is SessionDataContainer sessionData
				? (DateTime)sessionData.GetOrAdd(nameof(AntiViolenceBlocker), key => DateTime.Now.AddDays(-1))
				: DateTime.MinValue;
		}

		internal void SetTimer(DateTime? time = null) {
			if ( _me.IsAuthenticated ) {
				var val = time ?? DateTime.Now;
				_me.SessionData()?.AddOrUpdate(nameof(AntiViolenceBlocker), val, (key, old) => val);
			}
		}

		internal void ClearTimer() {
			SetTimer(DateTime.MinValue);
		}
	}
}
