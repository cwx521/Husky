#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.using System.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Husky.EF;
using Microsoft.EntityFrameworkCore;

namespace Husky.CommonModules.Users.Data
{
	public partial class UserModuleDbContext : DbContext
	{
		public UserModuleDbContext(DbContextOptions<UserModuleDbContext> options) : base(options) {
		}

		//User
		public DbSet<User> Users { get; set; }
		//User HasOne
		public DbSet<UserPhone> UserPhones { get; set; }
		public DbSet<UserWeChat> UserWeChats { get; set; }
		//User HasMany
		public DbSet<UserAddress> UserAddresses { get; set; }
		public DbSet<UserCredit> UserCredits { get; set; }
		public DbSet<UserLoginRecord> UserLoginRecords { get; set; }
		public DbSet<UserMessage> UserMessage { get; set; }
		public DbSet<UserPassword> UserPasswords { get; set; }

		//Lookup
		public DbSet<CreditType> CreditTypes { get; set; }
		public DbSet<UserMessageCommonContent> UserMessageCommonContents { get; set; }

		protected override void OnModelCreating(ModelBuilder mb) {
			mb.ApplyHuskyAnnotations();

			//QueryFilters
			mb.Entity<UserPassword>().HasQueryFilter(x => !x.IsObsoleted);

			//User
			mb.Entity<User>(user => {
				user.HasOne(x => x.Phone).WithOne(x => x.User).HasForeignKey<UserPhone>(x => x.UserId);
				user.HasOne(x => x.WeChat).WithOne(x => x.User).HasForeignKey<UserWeChat>(x => x.UserId);
				user.HasMany(x => x.Credits).WithOne(x => x.User).HasForeignKey(x => x.UserId);
				user.HasMany(x => x.Passwords).WithOne(x => x.User).HasForeignKey(x => x.UserId);
				user.HasMany<UserAddress>().WithOne(x => x.User).HasForeignKey(x => x.UserId);
				user.HasMany<UserMessage>().WithOne(x => x.User).HasForeignKey(x => x.UserId);
				user.HasMany<UserLoginRecord>().WithOne(x => x.User).HasForeignKey(x => x.UserId);
			});

			//UserCredit
			mb.Entity<UserCredit>(credit => {
				credit.HasOne(x => x.CreditType).WithMany().HasForeignKey(x => x.CreditTypeId);
			});

			//UserMessage
			mb.Entity<UserMessage>(message => {
				message.HasOne(x => x.CommonContent).WithMany(x => x.UserMessages).HasForeignKey(x => x.CommonContentId);
			});
		}
	}
}
