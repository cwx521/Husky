#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

using Husky.NotificationTasks.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.FileStore.Data
{
	public class NotificationTaskDbContext : DbContext, INotificationTaskDbContext
	{
		public NotificationTaskDbContext(DbContextOptions<NotificationTaskDbContext> options) : base(options) {
		}

		public DbContext Normalize() => this;

		public DbSet<NotificationTask> NotificationTasks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyHuskyAnnotations(this);
		}
	}
}
