#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Users.Data
{
	public class UsersDbContext : DbContext, IUsersDbContext
	{
		public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<User> Users { get; set; }

		public DbSet<UserPhone> UserPhones { get; set; }
		public DbSet<UserEmail> UserEmails { get; set; }
		public DbSet<UserWeChat> UserWeChats { get; set; }
		public DbSet<UserWeChatOpenId> UserWeChatOpenIds { get; set; }
		public DbSet<UserReal> UserReals { get; set; }
		public DbSet<UserPassword> UserPasswords { get; set; }
		public DbSet<UserAddress> UserAddresses { get; set; }
		public DbSet<UserLoginRecord> UserLoginRecords { get; set; }

		public DbSet<UserGroup> UserGroups { get; set; }
		public DbSet<UserInGroup> UserInGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomizedAnnotations();
			modelBuilder.OnUsersDbModelCreating();
		}
	}
}
