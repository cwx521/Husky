using System;
using Husky.FileStore.Data;
using Husky.NotificationTasks;
using Husky.NotificationTasks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using INotificationTaskManager = Husky.NotificationTasks.INotificationTaskManager;

namespace Husky
{
	public static class DependencyInjection
	{
		public static HuskyServiceHub AddNotificationTasks(this HuskyServiceHub husky, Action<DbContextOptionsBuilder> optionsAction) {
			husky.Services.AddDbContextPool<INotificationTaskDbContext, NotificationTaskDbContext>(optionsAction);
			husky.Services.AddScoped<INotificationTaskManager, NotificationTaskManager>();
			return husky;
		}

		public static HuskyServiceHub AddNotificationTasks<TDbContext>(this HuskyServiceHub husky)
			where TDbContext : DbContext, INotificationTaskDbContext {
			husky.Services.AddDbContext<INotificationTaskDbContext, TDbContext>();
			husky.Services.AddScoped<INotificationTaskManager, NotificationTaskManager>();
			return husky;
		}
	}
}
