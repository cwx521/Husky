using Husky.Data;
using Husky.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Husky.Users.Data
{
	public class UserDbContext : DbContextBase
	{
		public UserDbContext(IDatabaseFinder finder) : base(finder) {
		}

		public DbSet<User> Users { get; set; }
		public DbSet<UserPersonal> UserPersonals { get; set; }
		public DbSet<UserLoginRecord> UserLoginRecords { get; set; }
		public DbSet<UserChangeRecord> UserChangeRecords { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.Entity<User>(user => {
				user.HasOne(x => x.Personal).WithOne(x => x.User).HasForeignKey<UserPersonal>(x => x.UserId).IsRequired().OnDelete(DeleteBehavior.Restrict);
				user.HasMany(x => x.LoginRecords).WithOne(x => x.User).HasForeignKey(x => x.UserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
				user.HasMany(x => x.ChangeRecords).WithOne(x => x.User).HasForeignKey(x => x.UserId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
			});
			base.OnModelCreating(modelBuilder);
		}
	}
}
