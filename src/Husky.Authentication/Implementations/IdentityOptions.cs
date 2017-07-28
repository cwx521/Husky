using System;
using Husky.Authentication.Abstractions;

namespace Husky.Authentication.Implementations
{
	public sealed class IdentityOptions<T> where T : IFormattable, IEquatable<T>
	{
		public string Key { get; set; }
		public string Token { get; set; }
		public bool SessionMode { get; set; } = true;
		public DateTimeOffset? Expires { get; set; }
		public IIdentityEncyptor<T> Encryptor { get; set; }

		internal IdentityOptions<T> SolveUnassignedOptions(IdentityCarrier carrier) {
			if ( string.IsNullOrEmpty(Key) ) {
				Key = "WEIXING_AUTH_IDENTITY";
			}
			if ( Expires == null && carrier != IdentityCarrier.Header ) {
				Expires = DateTimeOffset.Now.AddMinutes(30);
			}
			if ( Encryptor == null && carrier != IdentityCarrier.Session ) {
				Encryptor = new IdentityEncryptor<T>();
			}
			return this;
		}
	}
}