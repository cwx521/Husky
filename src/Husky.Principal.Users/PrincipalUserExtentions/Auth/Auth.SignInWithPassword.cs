using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users
{
	public partial class UserAuthFunctions
	{
		public async Task<Result> SignInWithPasswordAsync(string mobileOrEmail, string passwordClearText) {
			if ( string.IsNullOrEmpty(mobileOrEmail) || string.IsNullOrEmpty(passwordClearText) ) {
				return new Failure(LoginResult.InvalidInput.ToLabel());
			}

			var isMobile = mobileOrEmail.IsMainlandMobile();
			var isEmail = mobileOrEmail.IsEmail();

			if ( !isMobile && !isEmail ) {
				return new Failure(LoginResult.InvalidInput.ToLabel());
			}

			var query = isMobile
				? _db.Users.Where(x => x.Phone != null && x.Phone.Number == mobileOrEmail)
				: _db.Users.Where(x => x.Email != null && x.Email.EmailAddress == mobileOrEmail);

			var user = await query.Include(x => x.Passwords).SingleOrDefaultAsync();
			if ( user == null ) {
				return await AddLoginRecordAsync(LoginResult.AccountNotFound, mobileOrEmail, null, passwordClearText);
			}

			//todo: consider to add configuration
			const int withinMinutes = 10;
			const int allowAttemptTimes = 5;
			if ( IsNeedToSuspendFurtherLoginAttemption(user.Id, TimeSpan.FromMinutes(withinMinutes), allowAttemptTimes) ) {
				return await AddLoginRecordAsync(LoginResult.RejectedContinuousAttemption, mobileOrEmail, user.Id);
			}

			if ( user.Passwords.Count == 0 || user.Passwords.All(x => x.IsObsolete || x.Password != Crypto.SHA1(passwordClearText)) ) {
				return await AddLoginRecordAsync(LoginResult.ErrorPassword, mobileOrEmail, user.Id, passwordClearText);
			}
			if ( user.Status == RowStatus.Suspended ) {
				return await AddLoginRecordAsync(LoginResult.RejectedAccountSuspended, mobileOrEmail, user.Id);
			}
			if ( user.Status == RowStatus.Deleted ) {
				return await AddLoginRecordAsync(LoginResult.RejectedAccountDeleted, mobileOrEmail, user.Id);
			}
			if ( user.Status != RowStatus.Active ) {
				return await AddLoginRecordAsync(LoginResult.RejectedAccountInactive, mobileOrEmail, user.Id);
			}

			await _db.Normalize().SaveChangesAsync();

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName ?? mobileOrEmail.Mask()!;
			_me.IdentityManager?.SaveIdentity(_me);

			return await AddLoginRecordAsync(LoginResult.Success, mobileOrEmail, user.Id);
		}

		private bool IsNeedToSuspendFurtherLoginAttemption(int userId, TimeSpan withinTime, int maxAllowedAttemptionTimes = 5) {
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
