using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MD5Algorithm = System.Security.Cryptography.MD5;
using SHA1Algorithm = System.Security.Cryptography.SHA1;

namespace Husky
{
	public static class Crypto
	{
		#region PermanentToken

		private static string _permanentToken;

		public static string PermanentToken {
			get {
				if ( string.IsNullOrEmpty(_permanentToken) ) {
					throw new InvalidOperationException($"{nameof(Crypto)}.{nameof(PermanentToken)} has not been assigned yet, the value of it is still null or empty.");
				}
				return _permanentToken;
			}
			set => _permanentToken = value ?? throw new ArgumentNullException(nameof(value));
		}

		#endregion

		#region RandomBytes, RandomNumber, RandomString

		public static byte[] RandomBytes(int length = 4) {
			using ( var rng = RandomNumberGenerator.Create() ) {
				var salt = new byte[length];
				rng.GetBytes(salt);
				return salt;
			}
		}

		public static int RandomNumber() => BitConverter.ToInt32(RandomBytes(4), 0);

		public static string RandomString(int length = 8) {
			const string chars = "0123456789abcdefghijklmkopqrstuvwxyzABCDEFGHIJKLMKOPQRSTUVWXYZ";
			var salt = RandomBytes(length);
			var builder = new StringBuilder();
			for ( var i = 0; i < length; builder.Append(chars[salt[i++] % chars.Length]) ) ;
			return builder.ToString();
		}

		public static Random Random() => new Random(RandomNumber());

		#endregion

		#region MD5, SHA1

		public static string MD5(this string str) {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}
			using ( var algorithm = MD5Algorithm.Create() ) {
				var original = Encoding.UTF8.GetBytes(str);
				var encoded = algorithm.ComputeHash(original);
				return encoded.Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
			}
		}

		public static string SHA1(this string str) {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}
			using ( var algorithm = SHA1Algorithm.Create() ) {
				var original = Encoding.UTF8.GetBytes(str);
				var encoded = algorithm.ComputeHash(original);
				return encoded.Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
			}
		}

		#endregion

		#region HmacMD5, HmacSHA1

		public static string Hmac<TAlgorithm>(this string str, string key) where TAlgorithm : HMAC, new() {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}

			if ( key == null ) {
				throw new ArgumentNullException(nameof(key));
			}

			using ( var hmac = new TAlgorithm() ) {
				hmac.Key = ComputeKey(key);
				var original = Encoding.UTF8.GetBytes(str);
				var encoded = hmac.ComputeHash(original);
				return encoded.Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
			}
		}

		public static string HmacMD5(this string str, string key) => Hmac<HMACMD5>(str, key);
		public static string HmacSHA1(this string str, string key) => Hmac<HMACSHA1>(str, key);

		#endregion

		#region AES: Encrypt, Decrypt

		public static byte[] IV { get; set; } = { 0x6E, 0x70, 0x69, 0x64, 0x65, 0x78, 0x72, 0x65, 0x65, 0x50, 0x67, 0x6F, 0x77, 0x42, 0x79, 0x57 };

		public static string Encrypt(string str, string key = null) {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}

			using ( var aes = Aes.Create() ) {
				aes.Key = ComputeKey(key ?? PermanentToken);
				aes.IV = IV;
				using ( var encryptor = aes.CreateEncryptor() ) {
					var original = Encoding.UTF8.GetBytes(str);
					var encrypted = encryptor.TransformFinalBlock(original, 0, original.Length);
					return Convert.ToBase64String(encrypted).Mutate();
				}
			}
		}

		public static string Decrypt(string base64String, string key = null) {
			if ( base64String == null ) {
				throw new ArgumentNullException(nameof(base64String));
			}

			using ( var aes = Aes.Create() ) {
				aes.Key = ComputeKey(key ?? PermanentToken);
				aes.IV = IV;
				using ( var decryptor = aes.CreateDecryptor() ) {
					var base64 = Convert.FromBase64String(base64String.Restore());
					var decrypted = decryptor.TransformFinalBlock(base64, 0, base64.Length);
					return Encoding.UTF8.GetString(decrypted);
				}
			}
		}

		public static string Encrypt<T>(T obj, string key) where T : IFormattable => Encrypt(obj.ToString(), key);
		public static T Decrypt<T>(string base64String, string key) where T : IFormattable => Decrypt(base64String, key).As<T>();

		private static string Mutate(this string base64String) => base64String.Replace('+', '_').Replace('/', '-').TrimEnd('=');
		private static string Restore(this string base64String) => base64String.Replace('_', '+').Replace('-', '/').PadRight(base64String.Length + (base64String.Length % 4 == 0 ? 0 : (4 - base64String.Length % 4)), '=');

		#endregion

		#region ComputeKey

		private static byte[] ComputeKey(string givenKey) {
			using ( var md5 = MD5Algorithm.Create() ) {
				return md5.ComputeHash(Encoding.UTF8.GetBytes(givenKey));
			}
		}

		#endregion
	}
}