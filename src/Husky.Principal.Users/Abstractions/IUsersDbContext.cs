#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users.Data
{
	public interface IUsersDbContext
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

		DbSet<UserMessage> UserMessage { get; set; }
		DbSet<UserMessagePublicContent> UserMessagePublicContents { get; set; }
	}
}
