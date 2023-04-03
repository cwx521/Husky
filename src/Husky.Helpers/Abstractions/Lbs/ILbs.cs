using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs
{
	public interface ILbs
	{
		Task<LbsAddress?> GetAddressAsync(IPAddress ip);
		Task<LbsAddress?> GetAddressAsync(Location latlon);

		Task<Location?> GetLatLonAsync(string addressName);

		Task<Distance?> GetDistanceAsync(Location from, Location to, DistanceMode mode = DistanceMode.Driving);
		Task<Distance[]?> GetDistancesAsync(Location from, IEnumerable<Location> toMany, DistanceMode mode = DistanceMode.Driving);
	}
}
