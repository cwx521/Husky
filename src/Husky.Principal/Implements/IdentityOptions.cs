using System;

namespace Husky.Principal.Implements
{
	public sealed class IdentityOptions
	{
		public string Key { get; set; } = "HUSKY_AUTH_IDENTITY";
		public string Token { get; set; } = Crypto.PermanentToken;
		public bool SessionMode { get; set; } = true;
		public DateTimeOffset? Expires { get; set; }
		public IIdentityEncyptor Encryptor { get; set; } = new IdentityEncryptor();
	}
}