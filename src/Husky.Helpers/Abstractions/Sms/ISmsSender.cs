using System.Threading.Tasks;

namespace Husky.Sms
{
	public interface ISmsSender
	{
		Task SendAsync(ISmsBody sms, params string[] toMobileNumbers);
	}
}
