using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs
{
	public interface ILbs
	{
		Task<IAddress?> GetAddress(IPAddress ip);
		Task<IAddress?> GetAddress(ILatLon latlon);

		Task<ILatLon?> GetLatLon(string address);

		Task<IDistance?> GetDistance(ILatLon from, ILatLon to, DistanceMode mode = DistanceMode.Driving);
		Task<IDistance[]?> GetDistances(ILatLon from, IEnumerable<ILatLon> toMany, DistanceMode mode = DistanceMode.Driving);

		Task<ILatLon?> ConvertToTencentLatLon(ILatLon latlon);
		Task<ILatLon?> ConvertToBaiduLatLon(ILatLon latlon);
	}
}
