using System.Threading.Tasks;

namespace Husky.Sms.Abstractions
{
	public interface ISmsSender
	{
		Task SendAsync(string content, params string[] recipients);
	}
}
