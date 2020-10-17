//APIs document: https://lbs.qq.com/service/webService/webServiceGuide/webServiceGcoder

using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Husky.Lbs.QQLbs
{
	public class QQLbsService : ILbs
	{
		public QQLbsService(string key) {
			if ( string.IsNullOrEmpty(key) ) {
				throw new ArgumentNullException(nameof(key));
			}
			_settings = new QQLbsSettings {
				Key = key
			};
		}

		public QQLbsService(QQLbsSettings settings) {
			if ( settings == null ) {
				throw new ArgumentNullException(nameof(settings));
			}
			_settings = settings;
		}

		private readonly QQLbsSettings _settings;

		public async Task<GeoLocation?> QueryFromIp(IPAddress ip) {
			using var client = new WebClient();

			var ipString = ip.MapToIPv4().ToString();
			var url = $"{_settings.QueryIpApi}?key={_settings.Key}&ip={ipString}";
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
