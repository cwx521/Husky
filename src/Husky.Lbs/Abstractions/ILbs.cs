using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs
{
	public interface ILbs
	{
		Task<GeoLocation?> QueryFromIp(IPAddress ip);
	}
}
