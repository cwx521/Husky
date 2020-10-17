using System;

namespace Husky.Principal.Implements
{
	public sealed class IdentityOptions
	{
		public IdentityCarrier Carrier { get; set; } = IdentityCarrier.Cookie;
		public string Key { get; set; } = "HUSKY_AUTH_IDENTITY";
		public string Token { get; set; } = Crypto.PermanentToken;
		public bool SessionMode { get; set; } = true;
		public bool DedicateAnonymousIdStorage { get; set; } = false;
		public DateTimeOffset? Expires { get; set; }
		public IIdentityEncyptor Encryptor { get; set; } = new IdentityEncryptor();
	}
}