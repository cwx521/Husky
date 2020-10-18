//APIs document: https://lbs.qq.com/service/webService/webServiceGuide/webServiceGcoder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Husky.Lbs.QQLbs
{
	public class QQLbsService : ILbs
	{
		public QQLbsService(string key) {
			_settings = new QQLbsSettings();
			_settings.Key = key ?? throw new ArgumentNullException(nameof(key));
		}

		public QQLbsService(QQLbsSettings settings) {
			_settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		private readonly QQLbsSettings _settings;

		public async Task<IAddress?> GetAddress(IPAddress ip) {
			var ipString = ip.MapToIPv4().ToString();
			var url = "https://apis.map.qq.com/ws/location/v1/ip" + $"?key={_settings.Key}&ip={ipString}";
			var x = await GetApiResult(url);

			return x == null ? null : new Address {
				Lat = x.location.lat,
				Lon = x.location.lng,
				LatLonType = LatLonType.Tencent,

				Province = x.ad_info.province,
				City = x.ad_info.city,
				District = x.ad_info.district,
				ZipCode = x.adcode
			};

		}

		public async Task<IAddress?> GetAddress(ILatLon latlon) {
			var url = "https://apis.map.qq.com/ws/geocoder/v1/" + $"?key={_settings.Key}&location={latlon.Lat},{latlon.Lon}";
			var x = await GetApiResult(url);

			return x == null ? null : new Address {
				Lat = x.ad_info.location.lat,
				Lon = x.ad_info.location.lng,
				LatLonType = LatLonType.Tencent,

				DisplayAddress = x.address,
				DisplayAddressAlternate = x.formatted_addresses?.recommend,

				Province = x.address_component.province,
				City = x.address_component.city,
				District = x.address_component.district,
				Street = x.address_component.street,
				StreetNumber = x.address_component.street_number,
				ZipCode = x.ad_info.adcode
			};
		}

		public async Task<ILatLon?> GetLatLon(string address) {
			var url = "https://apis.map.qq.com/ws/geocoder/v1/" + $"?key={_settings.Key}&address={address}";
			var x = await GetApiResult(url);

			return x == null ? (ILatLon?)null : new LatLon {
				Lat = x.location.lat,
				Lon = x.location.lng,
				LatLonType = LatLonType.Tencent
			};
		}

		public async Task<IDistance?> GetDistance(ILatLon from, ILatLon to, DistanceMode mode = DistanceMode.Driving) {
			if ( from.LatLonType != to.LatLonType ) {
				throw new ArgumentException("坐标系不一致");
			}
			if ( mode == DistanceMode.Straight ) {
				return to.StraightDistanceTo(from);
			}

			var url = "https://apis.map.qq.com/ws/distance/v1/matrix/" +
					  $"?key={_settings.Key}" +
					  $"&mode={mode.ToLower()}" +
					  $"&from={from.Lat},{from.Lon}" +
					  $"&to={to.Lat},{to.Lon}";

			var x = await GetApiResult(url);

			return x == null ? null : new Distance {
				From = from,
				To = to,
				Mode = mode,
				Meters = (int)x.rows[0].elements[0].distance,
				TravelTimeEstimate = TimeSpan.FromSeconds((int?)x.rows[0].elements[0].duration ?? 0),
			};
		}

		public async Task<IDistance[]?> GetDistances(ILatLon from, IEnumerable<ILatLon> toMany, DistanceMode mode = DistanceMode.Driving) {
			if ( toMany.Any(x => x.LatLonType != from.LatLonType) ) {
				throw new ArgumentException("坐标系不一致");
			}

			if ( mode == DistanceMode.Straight ) {
				return toMany.Select(to => to.StraightDistanceTo(from)).ToArray();
			}

			var url = "https://apis.map.qq.com/ws/distance/v1/matrix/" +
					  $"?key={_settings.Key}&mode={mode.ToLower()}&from={from.Lat},{from.Lon}&to={string.Join(';', toMany.Select(x => x.ToString()))}";

			var x = await GetApiResult(url);

			if ( x == null ) {
				return null;
			}

			var n = toMany.Count();
			var i = 0;
			var results = new IDistance[n];
			foreach ( var to in toMany ) {
				results[i] = new Distance {
					From = from,
					To = to,
					Mode = mode,
					Meters = x.rows[0].elements[i].distance,
					TravelTimeEstimate = TimeSpan.FromSeconds((int?)x.rows[0].elements[i].duration ?? 0),
				};
				i++;
			}
			return results;
		}

		public async Task<ILatLon?> ConvertToTencentLatLon(ILatLon latlon) {
			if ( (int)latlon.LatLonType == (int)LatLonType.Tencent ) {
				latlon.LatLonType = LatLonType.Tencent;
				return latlon;
			}

			using var client = new WebClient();
			var url = "https://apis.map.qq.com/ws/coord/v1/translate" + $"?key={_settings.Key}&type={(int)latlon.LatLonType}&locations={latlon.Lat},{latlon.Lon}";

			try {
				var json = await client.DownloadStringTaskAsync(url);
				var x = JsonConvert.DeserializeObject<dynamic>(json);

				if ( x.status == 0 ) {
					return new LatLon {
						Lat = x.locations[0].lat,
						Lon = x.locations[0].lng,
						LatLonType = LatLonType.Tencent
					};
				}
			}
			catch {
			}
			return null;
		}

		public async Task<ILatLon?> ConvertToBaiduLatLon(ILatLon latlon) {
			if ( (int)latlon.LatLonType == (int)LatLonType.Baidu ) {
				latlon.LatLonType = LatLonType.Baidu;
				return latlon;
			}
			return await Task.Run(() => {
				var xPI = Math.PI * 3000 / 180;
				var z = Math.Sqrt(latlon.Lon * latlon.Lon + latlon.Lat * latlon.Lat) + 0.00002 * Math.Sin(latlon.Lat * xPI);
				var theta = Math.Atan2(latlon.Lat, latlon.Lon) + 0.000003 * Math.Cos(latlon.Lon * xPI);
				return new LatLon {
					Lat = (float)(z * Math.Sin(theta) + 0.006),
					Lon = (float)(z * Math.Cos(theta) + 0.0065),
					LatLonType = LatLonType.Baidu
				};
			});
		}

		private async Task<dynamic?> GetApiResult(string url) {
			using var client = new WebClient();

			try {
				var json = await client.DownloadStringTaskAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json);

				if ( d.status == 0 && d.message == "query ok" ) {
					return d.result;
				}
			}
			catch { }

			return null;
		}
	}
}
