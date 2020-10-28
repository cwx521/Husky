using System.Threading.Tasks;

namespace Husky.Sms
{
	public interface ISmsSender
	{
		Task<Result> SendAsync(ISmsBody sms, params string[] toMobileNumbers);
	}
}
