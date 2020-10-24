using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users
{
	public partial class UserAuthManager
	{
		public async Task<Result> SignInWithPassword(string mobileNumber, string passwordClearText) {
			if ( string.IsNullOrEmpty(mobileNumber) || string.IsNullOrEmpty(passwordClearText) ) {
				return new Failure(LoginResult.InvalidInput.ToLabel());
			}
			if ( !mobileNumber.IsMainlandMobile() ) {
				return new Failure(LoginResult.InvalidInput.ToLabel());
			}

			var user = _db.Users
				.Include(x => x.Passwords)
				.Where(x => x.Phone != null && x.Phone.Number == mobileNumber)
				.SingleOrDefault();

			if ( user == null ) {
				return await AddLoginRecord(LoginResult.AccountNotFound, mobileNumber, null, passwordClearText);
			}

			const int withinMinutes = 10;
			const int allowAttemptTimes = 5;
			if ( IsNeedToSuspendFurtherLoginAttemption(user.Id, TimeSpan.FromMinutes(withinMinutes), allowAttemptTimes) ) {
				return await AddLoginRecord(LoginResult.RejectedContinuousAttemption, mobileNumber, user.Id);
			}

			if ( user.Passwords.Count == 0 || user.Passwords.All(x => x.IsObsolete || x.Password != Crypto.SHA1(passwordClearText)) ) {
				return await AddLoginRecord(LoginResult.ErrorPassword, mobileNumber, user.Id, passwordClearText);
			}
			if ( user.Status == RowStatus.Suspended ) {
				return await AddLoginRecord(LoginResult.RejectedAccountSuspended, mobileNumber, user.Id);
			}
			if ( user.Status == RowStatus.Deleted ) {
				return await AddLoginRecord(LoginResult.RejectedAccountDeleted, mobileNumber, user.Id);
			}
			if ( user.Status != RowStatus.Active ) {
				return await AddLoginRecord(LoginResult.RejectedAccountInactive, mobileNumber, user.Id);
			}

			await _db.Normalize().SaveChangesAsync();

			_me.Id = user.Id;
			_me.DisplayName = user.DisplayName ?? mobileNumber.Mask()!;
			_me.IdentityManager?.SaveIdentity(_me);

			return await AddLoginRecord(LoginResult.Success, mobileNumber, user.Id);
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
