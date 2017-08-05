using System;
using System.Threading.Tasks;
using Husky.Sugar;
using Husky.Users.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.Users.Extensions
{
	partial class PrincipalUserExtensions
	{
		public async Task<Result<User>> SignUp(AccountNameType usingType, string accountName, string password, bool verified) {
			if ( string.IsNullOrEmpty(accountName) ) {
				return new Failure<User>("帐号不能为空。".Xslate());
			}
			if ( string.IsNullOrEmpty(password) ) {
				return new Failure<User>("密码不能为空。".Xslate());
			}

			var isEmail = accountName.IsEmail();
			var isMobile = accountName.IsMainlandMobile();

			if ( !isEmail && usingType == AccountNameType.Email ) {
				return new Failure<User>("{0} 必须是有效的邮箱地址。".Xslate(accountName));
			}
			if ( !isMobile && usingType == AccountNameType.Mobile ) {
				return new Failure<User>("{0} 必须是有效的手机号。".Xslate(accountName));
			}

			var isAccountTaken = isEmail
					? await _userDb.Users.AnyAsync(x => x.Email == accountName)
					: await _userDb.Users.AnyAsync(x => x.Mobile == accountName);
			if ( isAccountTaken ) {
				return new Failure<User>("{0} 已经被注册了。".Xslate(accountName));
			}

			var user = new User {
				Email = isEmail ? accountName : null,
				Mobile = isMobile ? accountName : null,
				IsEmailVerified = (isEmail && verified),
				IsMobileVerified = (isMobile && verified),
				Password = string.IsNullOrEmpty(password) ? null : Crypto.SHA1(password)
			};
			_userDb.Add(user);

			await _userDb.SaveChangesAsync();
			await AddLoginRecord(user, accountName, null, LoginResult.Success, "新注册。".Xslate());

			_my.IdString = user.Id.ToString();
			_my.DisplayName = user.DisplayName;
			_my.IdentityManager.SaveIdentity(_my);

			return new Success<User>(user);
		}

		public async Task<LoginResult> SignIn(string accountName, string password) {
			if ( string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(password) ) {
				return LoginResult.InvalidInput;
			}

			var user = accountName.IsEmail()
					? await _userDb.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Email == accountName)
					: await _userDb.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Mobile == accountName);

			if ( user == null ) {
				return await AddLoginRecord(user, accountName, null, LoginResult.AccountNotFound);
			}
			if ( user.Password != Crypto.SHA1(password) ) {
				return await AddLoginRecord(user, accountName, password, LoginResult.ErrorPassword);
			}
			if ( user.Status != RowStatus.Active ) {
				return await AddLoginRecord(user, accountName, null, LoginResult.RejectedAccountInactive);
			}

			const int withinMinutes = 10;
			const int allowAttemptTimes = 5;
			if ( _userDb.IsSuspendFurtherLoginAttemptionByFailureRecordsAnalysis(user.Id, TimeSpan.FromMinutes(withinMinutes), allowAttemptTimes) ) {
				return await AddLoginRecord(user, accountName, null, LoginResult.RejectedContinuousAttemption);
			}

			_my.IdString = user.Id.ToString();
			_my.DisplayName = user.DisplayName;
			_my.IdentityManager.SaveIdentity(_my);

			return await AddLoginRecord(user, accountName, null, LoginResult.Success);
		}

		public void SignOut() => _my.IdentityManager.DeleteIdentity();

		async Task<LoginResult> AddLoginRecord(User user, string inputAccount, string sickPassword, LoginResult result, string description = null) {
			_userDb.Add(new UserLoginRecord {
				LoginResult = result,
				UserId = user?.Id,
				InputAccount = inputAccount,
				SickPassword = sickPassword?.Length > 18 ? "(超出18位)".Xslate() : sickPassword,
				Description = description,
				UserAgent = _http.Request.Headers["User-Agent"],
				Ip = _http.Connection.RemoteIpAddress.ToString()
			});
			await _userDb.SaveChangesAsync();

			if ( result != LoginResult.Success ) {
				SignOut();
			}
			return result;
		}
	}
}
