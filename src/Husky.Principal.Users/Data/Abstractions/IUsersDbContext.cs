using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users.Data
{
	public interface IUsersDbContext : IDisposable, IAsyncDisposable
	{
		DbContext Normalize();

		DbSet<User> Users { get; set; }

		DbSet<UserPhone> UserPhones { get; set; }
		DbSet<UserEmail> UserEmails { get; set; }
		DbSet<UserWeChat> UserWeChats { get; set; }
		DbSet<UserWeChatOpenId> UserWeChatOpenIds { get; set; }
		DbSet<UserReal> UserReals { get; set; }
		DbSet<UserPassword> UserPasswords { get; set; }
		DbSet<UserAddress> UserAddresses { get; set; }
		DbSet<UserLoginRecord> UserLoginRecords { get; set; }

		DbSet<UserGroup> UserGroups { get; set; }
		DbSet<UserInGroup> UserInGroups { get; set; }
	}
}
