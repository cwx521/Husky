using System;
using System.Collections.Generic;
using System.Text;

namespace Husky.Principal.Implements
{
	internal static class IdentityAnalysisHelper
	{
		public static IIdentity GetIdentity(string anonymous, string logon, IdentityOptions options) {
			var identity = new Identity();
			identity.AnonymousId = anonymous.AsGuid(identity.AnonymousId);

			if ( string.IsNullOrEmpty(logon) ) {
				return identity;
			}
			var decrypted = options.Encryptor.Decrypt(logon, options.Token);
			if ( decrypted == null ) {
				return identity;
			}

			decrypted.AnonymousId = identity.AnonymousId;
			return decrypted;
		}
	}
}
