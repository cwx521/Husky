using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs
{
	public interface ILbs
	{
		Task<Address?> GetAddress(IPAddress ip);
		Task<Address?> GetAddress(Location latlon);

		Task<Location?> GetLatLon(string addressName);

		Task<Distance?> GetDistance(Location from, Location to, DistanceMode mode = DistanceMode.Driving);
		Task<Distance[]?> GetDistances(Location from, IEnumerable<Location> toMany, DistanceMode mode = DistanceMode.Driving);
	}
}
