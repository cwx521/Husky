using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Husky.NotificationTasks.Data;

namespace Husky.NotificationTasks
{
	internal static class NotificationTaskExecutionHelper
	{
		internal static async Task ExecuteAsync(this NotificationTask task, bool isAutomation = true) {
			if (isAutomation) {
				task.AutomatedCount++;
			}
			else {
				task.ManualAttemptedCount++;
			}

			try {
				var contentType = task.ContentType.ToLabel();
				var contentObject = new StringContent(task.ContentBody ?? string.Empty, Encoding.UTF8, contentType);
				var response = await HttpClientSingleton.Instance.PostAsync(task.ApiUrl, contentObject);
				var received = await response.Content.ReadAsStringAsync();

				//判定成功条件： 仅在接收到返回内容为 SUCCESS 或 OK 纯文本内容时
				if (received != null && new[] { "success", "ok" }.Contains(received.ToLower())) {
					task.Status = NotificationTaskStatus.Completed;
					task.ScheduleNextTime = null;
				}
				else if (isAutomation) {
					// mangic number here, consider to config this number somewhere
					task.Status = (task.AutomatedCount + task.ManualAttemptedCount) < 50 ? NotificationTaskStatus.Retry : NotificationTaskStatus.GivenUp;
					task.ScheduleNextTime = DateTime.Now.Add(PlanNextDuration(task.AutomatedCount));
				}

				task.ReceivedContent = received?.Left(4000);
				task.FirstTriedTime ??= DateTime.Now;
				task.LastTriedTime = DateTime.Now;
			}
			catch { }
		}

		private static TimeSpan PlanNextDuration(int automatedCount) => automatedCount switch {
			0 or 1 => TimeSpan.FromMilliseconds(500),
			2 or 3 or 4 => TimeSpan.FromMilliseconds(1000),
			5 => TimeSpan.FromSeconds(5),
			6 => TimeSpan.FromSeconds(15),
			7 => TimeSpan.FromSeconds(60),
			8 => TimeSpan.FromMinutes(5),
			9 => TimeSpan.FromMinutes(30),
			10 => TimeSpan.FromHours(1),
			_ => TimeSpan.FromHours(2),
		};
	}
}
