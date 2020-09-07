using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Husky;
using Husky.CommonModules.Users.Data;
using Husky.Principal.SessionData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal
{
	public sealed partial class AuthManager
	{
		internal AuthManager(IPrincipalUser principal) {
			_me = principal;
			_db = principal.ServiceProvider.GetRequiredService<UserModuleDbContext>();
			_http = principal.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
		}

		readonly IPrincipalUser _me;
		readonly UserModuleDbContext _db;
		readonly HttpContext _http;

		public async Task<Result> SignUpThroughPhone(string? mobileNumber, string? password, bool isMobileVerified = false) {
			if ( string.IsNullOrEmpty(mobileNumber) ) {
				return new Failure("请输入登录手机号");
			}
			if ( string.IsNullOrEmpty(password) ) {
				return new Failure("请输入密码");
			}
			if ( !mobileNumber.IsMainlandMobile() ) {
				return new Failure("请输入正确的登录手机号");
			}
			if ( _db.UserPhones.Any(x => x.Number == mobileNumber) ) {
				return new Failure($"{mobileNumber} 已被用于其他帐号");
			}

			var encryptedPassword = Crypto.SHA1(password);
			var user = new User {
				Phone = new UserPhone {
					Number = mobileNumber,
					IsVerified = isMobileVerified
				},
				Passwords = new List<UserPassword> {
					new UserPassword {
						Password = encryptedPassword
					}
				},
			};
			_db.Add(user);

			await _db.SaveChangesAsync();
			await AddLoginRecord(user.Id, mobileNumber, password, LoginResult.Success, "新注册");

			_me.Id = user.Id;
			_me.DisplayName = user.Phone.Number.Mask();
			_me.IdentityManager.SaveIdentity(_me);

			return new Success();
		}

		public async Task<LoginResult> SignIn(string mobileNumber, string password, string? additionalDescription = null) {
			if ( string.IsNullOrEmpty(mobileNumber) || string.IsNullOrEmpty(password) ) {
				return LoginResult.InvalidInput;
			}
			if ( !mobileNumber.IsMainlandMobile() ) {
				return LoginResult.InvalidInput;
			}

			var user = _db.Users
				.Include(x => x.Passwords)
				.Where(x => x.Phone != null && x.Phone.Number == mobileNumber)
				.SingleOrDefault();

			if ( user == null ) {
				return await AddLoginRecord(null, mobileNumber, password, LoginResult.AccountNotFound, additionalDescription);
			}

			const int withinMinutes = 10;
			const int allowAttemptTimes = 5;
			if ( IsNeedToSuspendFurtherLoginAttemptionByFailureRecordsAnalysis(user.Id, TimeSpan.FromMinutes(withinMinutes), allowAttemptTimes) ) {
				return await AddLoginRecord(user.Id, mobileNumber, null, LoginResult.RejectedContinuousAttemption);
			}

			if ( user.Passwords.All(x => x.IsObsoleted || x.Password != Crypto.SHA1(password)) ) {
				return await AddLoginRecord(user.Id, mobileNumber, password, LoginResult.ErrorPassword, additionalDescription);
			}
			if ( user.State == RowStatus.Suspended ) {
				return await AddLoginRecord(user.Id, mobileNumber, null, LoginResult.RejectedAccountSuspended, additionalDescription);
			}
			if ( user.State == RowStatus.DeletedByAdmin || user.State == RowStatus.DeletedByUser ) {
				return await AddLoginRecord(user.Id, mobileNumber, null, LoginResult.RejectedAccountDeleted, additionalDescription);
			}
			if ( user.State != RowStatus.Active ) {
				return await AddLoginRecord(user.Id, mobileNumber, null, LoginResult.RejectedAccountInactive, additionalDescription);
			}

			await _db.SaveChangesAsync();

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName ?? mobileNumber.Mask();
			_me.IdentityManager.SaveIdentity(_me);

			return await AddLoginRecord(user.Id, mobileNumber, password, LoginResult.Success, additionalDescription);
		}

		public void SignOut() {
			if ( _me.IsAuthenticated ) {
				_me.AbandonSessionData();
				_me.IdentityManager.DeleteIdentity();
				_me.Id = 0;
				_me.DisplayName = null;
			}
		}

		public async Task<LoginResult> AddLoginRecord(int? userId, string inputAccount, string? sickPassword, LoginResult result, string? description = null) {
			var ip = _http.Connection.RemoteIpAddress;
			var ipString = ip.MapToIPv4().ToString();

			var encryptedSickPassword = string.IsNullOrEmpty(sickPassword) || inputAccount.Length == 0
				? sickPassword
				: sickPassword.Length > 25
					? sickPassword.Left(88)
					: Crypto.Encrypt(sickPassword, ivSalt: inputAccount);

			_db.Add(new UserLoginRecord {
				LoginResult = result,
				UserId = userId,
				AttemptedAccount = inputAccount ?? "",
				SickPassword = encryptedSickPassword,
				Description = description,
				UserAgent = _http.Request.UserAgent(),
				Ip = ipString
			});
			await _db.SaveChangesAsync();

			if ( result != LoginResult.Success ) {
				SignOut();
			}
			return result;
		}

		bool IsNeedToSuspendFurtherLoginAttemptionByFailureRecordsAnalysis(int userId, TimeSpan withinTime, int maxAllowedAttemptionTimes = 5) {
			var list = _db.UserLoginRecords
				.AsNoTracking()
				.Where(x => x.UserId == userId)
				.Where(x => x.CreatedTime >= DateTime.Now.Subtract(withinTime))
				.Where(x => x.LoginResult != LoginResult.RejectedContinuousAttemption)
				.OrderByDescending(x => x.Id)
				.Select(x => x.LoginResult)
				.Take(maxAllowedAttemptionTimes)
				.ToList()
				;
			return list.Count == maxAllowedAttemptionTimes && list.All(x => x == LoginResult.ErrorPassword);
		}
	}
}
