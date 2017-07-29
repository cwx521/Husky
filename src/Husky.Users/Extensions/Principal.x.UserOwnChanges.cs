using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Sugar;
using Husky.Users.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.Users.Extensions
{
	partial class PrincipalUserExtensions
	{
		public async Task<Result> ChangeDisplayName(string newDisplayName) => await Change(nameof(User.DisplayName), newDisplayName);
		public async Task<Result> ChangeMobile(string newMobile) => await Change(nameof(User.Mobile), newMobile);
		public async Task<Result> ChangeEmail(string newEmail) => await Change(nameof(User.Email), newEmail);
		public async Task<Result> ChangePassword(string newPassword) => await Change(nameof(User.Password), Crypto.SHA1(newPassword));

		public async Task<Result> ChangeIsEmailVerified(bool isEmailVerified) => await Change(nameof(User.IsEmailVerified), isEmailVerified);
		public async Task<Result> ChangeIsMobileVerified(bool isMobileVerified) => await Change(nameof(User.IsMobileVerified), isMobileVerified);

		public async Task<Result> ChangeStatus(RowStatus newStatus) => await Change(nameof(User.Status), newStatus);
		public async Task<Result> SetStatusAwaitResetTime(TimeSpan timeSpan) => await Change(nameof(User.AwaitReactivateTime), DateTime.Now.Add(timeSpan));

		private async Task<Result> Change(string field, object value, bool allowNull = false) {
			if ( _my.IsAnonymous ) {
				return new Failure("您还没有登录。");
			}
			if ( (!allowNull && (value == null || value is string str && string.IsNullOrEmpty(str))) ) {
				return new Failure("不能空白。");
			}

			var isTaken = await _userDb.Users
				.Where(x => x.Id != _my.Id<Guid>())
				.Where(field, value, Comparison.Equal)
				.AnyAsync();

			if ( isTaken ) {
				return new Failure($"“{value}”正在被其他帐号使用。");
			}

			var entry = _userDb.ChangeTracker.Entries<User>().SingleOrDefault(x => x.Entity.Id == _my.Id<Guid>())
					 ?? _userDb.Attach(new User { Id = _my.Id<Guid>() });

			entry.Property(field).CurrentValue = value;
			await _userDb.SaveChangesAsync();

			if ( field == nameof(User.DisplayName) ) {
				_my.DisplayName = value as string;
			}
			else if ( value is RowStatus status && status != RowStatus.Active ) {
				SignOut();
			}
			return new Success();
		}
	}
}
