using System;
using Husky.Authentication.Abstractions;
using Husky.Sugar;

namespace Husky.Authentication.Implements
{
	public sealed class IdentityOptions
	{
		public string Key { get; set; }
		public string Token { get; set; }
		public bool SessionMode { get; set; } = true;
		public DateTimeOffset? Expires { get; set; }
		public IIdentityEncyptor Encryptor { get; set; }

		internal IdentityOptions SolveUnassignedOptions(IdentityCarrier carrier) {
			if ( string.IsNullOrEmpty(Key) ) {
				Key = "WEIXING_AUTH_IDENTITY";
			}
			if ( string.IsNullOrEmpty(Token) && carrier != IdentityCarrier.Session ) {
				Token = Crypto.PermanentToken;
			}
			if ( Expires == null && carrier != IdentityCarrier.Header ) {
				Expires = DateTimeOffset.Now.AddMinutes(30);
			}
			if ( Encryptor == null && carrier != IdentityCarrier.Session ) {
				Encryptor = new IdentityEncryptor();
			}
			return this;
		}
	}
}