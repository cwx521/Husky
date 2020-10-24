using System.Threading.Tasks;
using Husky.Principal.Users.Data;

namespace Husky.Principal.Users
{
	public partial class UserAuthManager
	{
		public async Task<Result> AddLoginRecord(LoginResult loginResult, string inputAccount, int? knownUserId = null, string? sickPassword = null) {
			var ip = _http.Connection.RemoteIpAddress;
			var ipString = ip.MapToIPv4().ToString();

			var encryptedSickPassword = string.IsNullOrEmpty(sickPassword) || inputAccount.Length == 0
				? sickPassword
				: sickPassword.Length > 25
					? sickPassword.Left(88)
					: Crypto.Encrypt(sickPassword, iv: inputAccount);

			_db.UserLoginRecords.Add(new UserLoginRecord {
				LoginResult = loginResult,
				UserId = knownUserId,
				AttemptedAccount = inputAccount ?? "",
				SickPassword = encryptedSickPassword,
				UserAgent = _http.Request.UserAgent(),
				Ip = ipString
			});

			await _db.Normalize().SaveChangesAsync();

			if ( loginResult != LoginResult.Success ) {
				SignOut();
				return new Failure(loginResult.ToLabel());
			}
			return new Success();
		}
	}
}
