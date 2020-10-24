using System.Threading.Tasks;
using Husky.Principal.Users.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Users
{
	public partial class UserAuthManager
	{
		public async Task<Result> AddLoginRecord(LoginResult loginResult, string? inputAccount, int? knownUserId = null, string? sickPassword = null) {
			var http = _me.ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

			var encryptedSickPassword = string.IsNullOrEmpty(sickPassword) || string.IsNullOrEmpty(inputAccount)
				? sickPassword
				: sickPassword.Length > 25
					? sickPassword.Left(88)
					: Crypto.Encrypt(sickPassword, iv: inputAccount);

			_db.UserLoginRecords.Add(new UserLoginRecord {
				LoginResult = loginResult,
				UserId = knownUserId,
				AttemptedAccount = inputAccount ?? "",
				SickPassword = encryptedSickPassword,
				UserAgent = http?.Request.UserAgent(),
				Ip = http?.Connection.RemoteIpAddress.MapToIPv4().ToString()
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
