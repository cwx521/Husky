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
		#region SecretToken

		private static string? _secretToken;

		public static string SecretToken {
			get {
				if ( string.IsNullOrEmpty(_secretToken) ) {
					throw new InvalidOperationException(
						$"{nameof(Crypto)}.{nameof(SecretToken)} is not been assigned yet which must not be null or empty. " +
						$"It is required to set this value for security purpose. \n" +
						$"(Simply set this by '{nameof(Crypto)}.{nameof(SecretToken)} = yourValue;' in Startup.cs)"
					);
				}
				return _secretToken;
			}
			set => _secretToken = value ?? throw new ArgumentNullException(nameof(value));
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
		public static long RandomInt64() => BitConverter.ToInt64(RandomBytes(8), 0);

		public static string RandomString(int length = 8) {
			const string chars = "0123456789abcdefghijklmkopqrstuvwxyzABCDEFGHIJKLMKOPQRSTUVWXYZ";
			var seed = RandomBytes(length);
			var builder = new StringBuilder();
			for ( var i = 0; i < length; builder.Append(chars[seed[i++] % chars.Length]) ) ;
			return builder.ToString();
		}

		#endregion

		#region MD5, SHA1, SHA256

		public static string MD5(this string str) {
			using var algorithm = MD5Algorithm.Create();
			return algorithm.GetHashedResult(str);
		}

		public static string SHA1(this string str) {
			using var algorithm = SHA1Algorithm.Create();
			return algorithm.GetHashedResult(str);
		}

		public static string SHA256(this string str) {
			using var algorithm = SHA256Algorithm.Create();
			return algorithm.GetHashedResult(str);
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
			return hmac.GetHashedResult(str);
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
			aes.Key = Hash(key ?? SecretToken);

			using var encryptor = aes.CreateEncryptor();
			var original = Encoding.UTF8.GetBytes(str);
			var encrypted = encryptor.TransformFinalBlock(original, 0, original.Length);
			return Convert.ToBase64String(encrypted).Mutate();
		}

		public static string Decrypt(string encrypted, string iv, string? key = null) {
			if ( encrypted == null ) {
				throw new ArgumentNullException(nameof(encrypted));
			}

			using var aes = Aes.Create();
			aes.IV = Hash(iv);
			aes.Key = Hash(key ?? SecretToken);

			using var decryptor = aes.CreateDecryptor();
			var base64 = Convert.FromBase64String(encrypted.Restore());
			var decrypted = decryptor.TransformFinalBlock(base64, 0, base64.Length);
			return Encoding.UTF8.GetString(decrypted);
		}

		public static string Encrypt<T>(T obj, string iv, string? key = null) where T : struct => Encrypt(obj.ToString()!, iv, key);
		public static T Decrypt<T>(string encrypted, string iv, string? key = null) where T : struct => Decrypt(encrypted, iv, key).As<T>();

		private static string Mutate(this string encrypted) => encrypted.Replace('+', '_').Replace('/', '-').TrimEnd('=');
		private static string Restore(this string encrypted) => encrypted.Replace('_', '+').Replace('-', '/').PadRight(encrypted.Length + (encrypted.Length % 4 == 0 ? 0 : (4 - encrypted.Length % 4)), '=');

		#endregion

		#region Private Members

		private static byte[] Hash(string givenKey) {
			using var md5 = MD5Algorithm.Create();
			return md5.ComputeHash(Encoding.UTF8.GetBytes(givenKey));
		}

		private static string GetHashedResult(this HashAlgorithm algorithm, string str) {
			if ( str == null ) {
				throw new ArgumentNullException(nameof(str));
			}
			var original = Encoding.UTF8.GetBytes(str);
			var encoded = algorithm.ComputeHash(original);
			return encoded.Aggregate(new StringBuilder(), (sb, i) => sb.Append(i.ToString("x2"))).ToString();
		}

		#endregion
	}
}