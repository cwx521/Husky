using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Husky;
using Husky.NotificationTasks.Data;
using Microsoft.Extensions.Hosting;
using Timer = System.Threading.Timer;

namespace Husky.NotificationTasks
{
	public class NotificationTaskService : IHostedService, IDisposable
	{
		public NotificationTaskService(INotificationTaskDbContext db) {
			_db = db;
		}

		private readonly int _taskIntervalMilliseconds = 200;
		private readonly INotificationTaskDbContext _db;

		private Timer? _timer;

		public static bool Paused { get; set; }

		public Task StartAsync(CancellationToken cancellationToken) {
			ScheduleNext();
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			return _timer != null
				? _timer.DisposeAsync().AsTask()
				: Task.CompletedTask;
		}

		private void ScheduleNext() {
			_timer = new Timer(
				async (state) => await DoWork(),
				null,
				TimeSpan.FromMilliseconds(_taskIntervalMilliseconds),
				TimeSpan.Zero
			);
		}

		private async Task DoWork() {
			if (!Paused) {
				var task = _db.NotificationTasks
					.Where(x => x.Status == NotificationTaskStatus.Pending || x.Status == NotificationTaskStatus.Retry)
					.Where(x => x.ScheduleNextTime.HasValue)
					.OrderBy(x => x.ScheduleNextTime)
					.ThenBy(x => x.Id)
					.FirstOrDefault();

				if (task != null) {
					task.ExecuteAsync().ConfigureAwait(false).GetAwaiter().GetResult();
					await _db.SaveChangesAsync();
				}
			}
			ScheduleNext();
		}

		public void Dispose() {
			_timer?.Dispose();
			_db.Dispose();
		}

		~NotificationTaskService() {
			Dispose();
		}
	}
}
