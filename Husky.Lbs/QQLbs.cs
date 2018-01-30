using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Husky.Lbs
{
	public class QQLbs : ILbs
	{
		public QQLbs(string key) {
			if ( string.IsNullOrEmpty(key) ) {
				throw new ArgumentNullException(nameof(key));
			}
			_key = key;
		}

		readonly string _key;

		public async Task<GeoLocation> Query(IPAddress ip) {
			using ( var client = new WebClient() ) {
				var ipString = ip.MapToIPv4().ToString();
				var url = $"http://apis.map.qq.com/ws/location/v1/ip?key={_key}&ip={ipString}";

				var json = await client.DownloadStringTaskAsync(url);
				if ( json == null ) {
					return null;
				}

				dynamic d = JsonConvert.DeserializeObject(json);
				if ( d.status != 0 ) {
					return null;
				}

				return new GeoLocation {
					Ip = ipString,
					Lon = d.location.lng,
					Lat = d.location.lat,
					Nation = d.ad_info.nation,
					Province = d.ad_info.province,
					City = d.ad_info.city,
					District = d.ad_info.district,
				};
			}
		}
	}
}
