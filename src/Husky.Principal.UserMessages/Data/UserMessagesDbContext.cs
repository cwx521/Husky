#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.UserMessages.Data
{
	public class UserMessagesDbContext : DbContext, IUserMessagesDbContext
	{
		public UserMessagesDbContext(DbContextOptions<UserMessagesDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<UserMessage> UserMessage { get; set; }
		public DbSet<UserMessagePublicContent> UserMessagePublicContents { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyAdditionalCustomizedSqlServerAnnotations(this);
			modelBuilder.OnUserMessagesDbModelCreating();
		}
	}
}
