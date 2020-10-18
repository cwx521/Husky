using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs
{
	public interface ILbs
	{
		Task<IAddress?> GetAddress(IPAddress ip);
		Task<IAddress?> GetAddress(LatLon latlon);

		Task<LatLon?> GetLatLon(string address);

		Task<IDistance?> GetDistance(LatLon from, LatLon to, DistanceMode mode = DistanceMode.Driving);
		Task<IDistance[]?> GetDistances(LatLon from, IEnumerable<LatLon> toMany, DistanceMode mode = DistanceMode.Driving);

		Task<LatLon?> ConvertToTencentLatLon(LatLon latlon);
		Task<LatLon?> ConvertToBaiduLatLon(LatLon latlon);
	}
}
