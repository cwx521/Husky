// temperary: need to be replaced by the file from Husky.Helpers

using System.Threading.Tasks;

namespace Husky.NotificationTasks
{
	public interface INotificationTaskManager
	{
		Task AddNewAsync(string apiUrl, string contentBody, PostContentType contentType = PostContentType.Json);
		Task ExecuteManuallyAsync(int taskId);
		Task AbortAsync(int taskId);
	}

	public enum PostContentType
	{
		[Label("application/json")]
		Json,

		[Label("text/xml")]
		Xml,

		[Label("text/plain")]
		PlainText
	}
}
