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

		public QQLbs(QQLbsSettings settings) {
			if ( settings == null ) {
				throw new ArgumentNullException(nameof(settings));
			}
			_key = settings.Key;
		}

		private readonly string _key;

		public async Task<GeoLocation?> Query(IPAddress ip) {
			using var client = new WebClient();

			var ipString = ip.MapToIPv4().ToString();
			var url = $"http://apis.map.qq.com/ws/location/v1/ip?key={_key}&ip={ipString}";

			var json = await client.DownloadStringTaskAsync(url);
			if ( json == null ) {
				return null;
			}

			var d = JsonConvert.DeserializeObject<dynamic>(json);
			if ( d == null ) {
				return null;
			}
			if ( d.status != 0 || d.message != "query ok" || d.result == null ) {
				return null;
			}

			var x = d.result;
			return new GeoLocation {
				Ip = x.ip,
				Lon = x.location.lng,
				Lat = x.location.lat,
				Nation = x.ad_info.nation,
				Province = x.ad_info.province,
				City = x.ad_info.city,
				District = x.ad_info.district,
			};
		}
	}
}
