using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MD5Algorithm = System.Security.Cryptography.MD5;
using SHA1Algorithm = System.Security.Cryptography.SHA1;
using SHA256Algorithm = System.Security.Cryptography.SHA256;

namespace Husky
{
	public static class Crypto
	{
		#region PermanentToken

		private static string? _permanentToken;

		public static string PermanentToken {
			get {
				if ( string.IsNullOrEmpty(_permanentToken) ) {
					throw new InvalidOperationException(
						$"{nameof(Crypto)}.{nameof(PermanentToken)} is still null or empty and has not been assigned yet. " +
						$"It is required to set this value for security purpose. \n" +
						$"(Simply set this by '{nameof(Crypto)}.{nameof(PermanentToken)} = yourValue;')"
					);
				}
				return _permanentToken;
			}
			set => _permanentToken = value ?? throw new ArgumentNullException(nameof(value));
		}

		#endregion

		#region RandomBytes, RandomNumber, RandomString

		public static byte[] RandomBytes(int length = 4) {
			using var rng = RandomNumberGenerator.Create();
			var result = new byte[length];
			rng.GetBytes(result);
			return result;
		}

		public static int RandomInt32() => BitConverter.ToInt32(RandomBytes(4), 0);
		public static int RandomInt64() => BitConverter.ToInt32(RandomBytes(8), 0);

		public static string RandomString(int length = 8) {
			const string chars = "0123456789abcdefghijklmkopqrstuvwxyzABCDEFGHIJKLMKOPQRSTUVWXYZ";
			var salt = RandomBytes(length);
			var builder = new StringBuilder();
			for ( var i = 0; i < length; builder.Append(chars[salt[i++] % chars.Length]) ) ;
			return builder.ToString();
		}

		#endregion

		#region MD5, SHA1, SHA256

		public static string MD5(this string str) {
			using var algorithm = MD5Algorithm.Create();
			return algorithm.GetStringResult(str);
		}

		public static string SHA1(this string str) {
			using var algorithm = SHA1Algorithm.Create();
			return algorithm.GetStringResult(str);
		}

		public static string SHA256(this string str) {
			using var algorithm = SHA256Algorithm.Create();
			return algorithm.GetStringResult(str);
		}

		private static string GetStringResult(this HashAlgorithm algorithm, string str) {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}
			var original = Encoding.UTF8.GetBytes(str);
			var encoded = algorithm.ComputeHash(original);
			return encoded.Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
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
			using var hmac = new TAlgorithm {
				Key = Hash(key)
			};
			return hmac.GetStringResult(str);
		}

		public static string HmacMD5(this string str, string key) => Hmac<HMACMD5>(str, key);
		public static string HmacSHA1(this string str, string key) => Hmac<HMACSHA1>(str, key);
		public static string HmacSHA256(this string str, string key) => Hmac<HMACSHA256>(str, key);

		#endregion

		#region AES: Encrypt, Decrypt

		public static string Encrypt(string str, string iv, string? key = null) {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}

			using var aes = Aes.Create();
			aes.IV = Hash(iv);
			aes.Key = Hash(key ?? PermanentToken);

			using var encryptor = aes.CreateEncryptor();
			var original = Encoding.UTF8.GetBytes(str);
			var encrypted = encryptor.TransformFinalBlock(original, 0, original.Length);
			return Convert.ToBase64String(encrypted).Mutate();
		}

		public static string Decrypt(string base64String, string iv, string? key = null) {
			if ( base64String == null ) {
				throw new ArgumentNullException(nameof(base64String));
			}

			using var aes = Aes.Create();
			aes.IV = Hash(iv);
			aes.Key = Hash(key ?? PermanentToken);

			using var decryptor = aes.CreateDecryptor();
			var base64 = Convert.FromBase64String(base64String.Restore());
			var decrypted = decryptor.TransformFinalBlock(base64, 0, base64.Length);
			return Encoding.UTF8.GetString(decrypted);
		}

		public static string Encrypt<T>(T obj, string iv, string? key) where T : struct => Encrypt(obj.ToString()!, iv, key);
		public static T Decrypt<T>(string base64String, string iv, string? key) where T : struct => Decrypt(base64String, iv, key).As<T>();

		private static string Mutate(this string base64String) => base64String.Replace('+', '_').Replace('/', '-').TrimEnd('=');
		private static string Restore(this string base64String) => base64String.Replace('_', '+').Replace('-', '/').PadRight(base64String.Length + (base64String.Length % 4 == 0 ? 0 : (4 - base64String.Length % 4)), '=');

		#endregion

		#region ComputeKey

		private static byte[] Hash(string givenKey) {
			using var md5 = MD5Algorithm.Create();
			return md5.ComputeHash(Encoding.UTF8.GetBytes(givenKey));
		}

		#endregion
	}
}