using System;
using Microsoft.EntityFrameworkCore;

namespace Husky.NotificationTasks.Data
{
	public interface INotificationTaskDbContext : IDbContext, IDisposable, IAsyncDisposable
	{
		DbSet<NotificationTask> NotificationTasks { get; set; }
	}
}
