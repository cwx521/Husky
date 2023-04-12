using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.GridQuery;
using Husky.NotificationTasks.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.NotificationTasks
{
	public class NotificationTaskManager : INotificationTaskManager
	{
		public NotificationTaskManager(INotificationTaskDbContext db) {
			_db = db;
		}

		private readonly INotificationTaskDbContext _db;

		public async Task<SuccessQueryResult<NotificationTask>> ListAsync(QueryCriteria? criteria = null) {
			return await _db.NotificationTasks.AsNoTracking().OrderByDescending(x => x.Id).ToResultAsync(criteria);
		}

		public async Task AddNewAsync(string apiUrl, string contentBody, PostContentType contentType = PostContentType.Json) {
			var task = new NotificationTask {
				ApiUrl = apiUrl,
				ContentBody = contentBody,
				ContentType = contentType,
				Status = NotificationTaskStatus.Pending,
				ScheduleNextTime = DateTime.Now,
				CreatedTime = DateTime.Now
			};
			_db.NotificationTasks.Add(task);
			await _db.SaveChangesAsync();
		}

		public async Task ExecuteManuallyAsync(int taskId) {
			var task = _db.NotificationTasks.Find(taskId);
			if (task == null || task.Status == NotificationTaskStatus.Completed) {
				return;
			}
			task.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
			await _db.SaveChangesAsync();
		}

		public async Task AbortAsync(int taskId) {
			var task = _db.NotificationTasks.Find(taskId);
			if (task == null || task.Status == NotificationTaskStatus.Completed) {
				return;
			}
			task.Status = NotificationTaskStatus.Aborted;
			await _db.SaveChangesAsync();
		}
	}
}
