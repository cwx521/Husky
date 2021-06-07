#pragma warning disable CS8602 // Dereference of a possibly null reference.

using Husky.Lbs;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users.Data
{
	public static class UsersDbModelBuilderHelper
	{
		public static void OnUsersDbModelCreating(this ModelBuilder mb) {

			//PrimaryKeys
			mb.Entity<UserInGroup>().HasKey(x => new { x.UserId, x.GroupId });

			//QueryFilters
			mb.Entity<UserPassword>().HasQueryFilter(x => !x.IsObsolete);
			mb.Entity<UserAddress>().HasQueryFilter(x => x.Status == RowStatus.Active &&
				x.City != null &&
				x.City.Length != 0 &&
				x.ContactName != null &&
				x.ContactName.Length != 0);

			//User
			mb.Entity<User>(user => {
				user.HasOne(x => x.Phone).WithOne(x => x.User).HasForeignKey<UserPhone>(x => x.UserId);
				user.HasOne(x => x.Email).WithOne(x => x.User).HasForeignKey<UserEmail>(x => x.UserId);
				user.HasOne(x => x.WeChat).WithOne(x => x.User).HasForeignKey<UserWeChat>(x => x.UserId);
				user.HasOne(x => x.Real).WithOne(x => x.User).HasForeignKey<UserReal>(x => x.UserId);
				user.HasMany(x => x.Passwords).WithOne(x => x.User).HasForeignKey(x => x.UserId);
				user.HasMany(x => x.InGroups).WithOne(x => x.User).HasForeignKey(x => x.UserId);
				user.HasMany(x => x.Addresses).WithOne(x => x.User).HasForeignKey(x => x.UserId);
			});

			//UserWeChat
			mb.Entity<UserWeChat>(wechat => {
				wechat.HasMany(x => x.OpenIds).WithOne(x => x.WeChat).HasForeignKey(x => x.WeChatId);
			});

			//UserGroup
			mb.Entity<UserInGroup>(userInGroup => {
				userInGroup.HasOne(x => x.Group).WithMany().HasForeignKey(x => x.GroupId);
			});

			//UserLoginRecord
			mb.Entity<UserLoginRecord>(record => {
				record.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
			});
		}
	}
}
