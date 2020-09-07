using System;
using Husky.Principal.SessionData;

namespace Husky.Principal
{
	public static partial class PrincipalExtensions
	{
		public static DateTime AntiViolenceTimer(this IPrincipalUser principal) {
			var sessionData = principal.SessionData();
			if ( sessionData == null ) {
				return DateTime.MinValue;
			}
			return (DateTime)sessionData.GetOrAdd(nameof(AntiViolenceTimer), key => DateTime.Now.AddDays(-1));
		}

		public static void SetAntiViolenceTimer(this IPrincipalUser principal, DateTime? time = null) {
			if ( principal.IsAnonymous ) {
				return;
			}
			var val = time ?? DateTime.Now;
			principal.SessionData()?.AddOrUpdate(nameof(AntiViolenceTimer), val, (key, old) => val);
		}

		public static void ClearAntiViolenceTimer(this IPrincipalUser principal) {
			SetAntiViolenceTimer(principal, DateTime.MinValue);
		}
	}
}
