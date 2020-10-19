using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Husky.Lbs
{
	public interface ILbs
	{
		Task<IAddress?> GetAddress(IPAddress ip);
		Task<IAddress?> GetAddress(Location latlon);

		Task<Location?> GetLatLon(string address);

		Task<IDistance?> GetDistance(Location from, Location to, DistanceMode mode = DistanceMode.Driving);
		Task<IDistance[]?> GetDistances(Location from, IEnumerable<Location> toMany, DistanceMode mode = DistanceMode.Driving);
	}
}
