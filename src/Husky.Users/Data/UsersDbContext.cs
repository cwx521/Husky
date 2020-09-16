#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Husky.EF;
using Microsoft.EntityFrameworkCore;

namespace Husky.Users.Data
{
	public partial class UsersDbContext : DbContext
	{
		public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) {
		}

		//User
		public DbSet<User> Users { get; set; }
		//User HasOne
		public DbSet<UserPhone> UserPhones { get; set; }
		public DbSet<UserWeChat> UserWeChats { get; set; }
		public DbSet<UserWeChatOpenId> UserWeChatOpenIds { get; set; }
		public DbSet<UserReal> UserReals { get; set; }
		//User HasMany
		public DbSet<UserPassword> UserPasswords { get; set; }
		public DbSet<UserAddress> UserAddresses { get; set; }
		public DbSet<UserLoginRecord> UserLoginRecords { get; set; }
		public DbSet<UserMessage> UserMessage { get; set; }
		public DbSet<UserMessageCommonContent> UserMessageCommonContents { get; set; }

		protected override void OnModelCreating(ModelBuilder mb) {
			mb.ApplyHuskyAnnotations();
			mb.OnUsersModelCreating();
		}
	}
}
