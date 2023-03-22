using System;

namespace Husky.Principal
{
	public interface IIdentityOptions
	{
		IdentityCarrier Carrier { get; set; }
		string IdKey { get; set; }
		string AnonymousIdKey { get; set; }
		string Token { get; set; }
		bool SessionMode { get; set; }
		bool DedicateAnonymousIdStorage { get; set; }
		DateTimeOffset? Expires { get; set; }
		IIdentityEncyptor Encryptor { get; set; }
	}
}
