using System.Threading.Tasks;
using Husky.GridQuery;

namespace Husky.NotificationTasks
{
	public interface INotificationTaskManager
	{
		Task AddNewAsync(string apiUrl, string contentBody, PostContentType contentType = PostContentType.Json);
		Task ExecuteManuallyAsync(int taskId);
		Task AbortAsync(int taskId);
	}
}
