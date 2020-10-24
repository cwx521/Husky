//APIs document: https://lbs.qq.com/service/webService/webServiceGuide/webServiceGcoder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
		private readonly HttpClient _httpClient = new HttpClient();

		public async Task<Address?> GetAddressAsync(IPAddress ip) {
			var ipString = ip.MapToIPv4().ToString();
			var url = "https://apis.map.qq.com/ws/location/v1/ip" + $"?key={_settings.Key}&ip={ipString}";
			var x = await GetApiResultAsync(url);

			return x == null ? null : new Address {
				Location = new Location {
					Lat = x.location.lat,
					Lon = x.location.lng,
					LatLonType = LatLonType.Tencent
				},
				Province = x.ad_info.province,
				City = x.ad_info.city,
				District = x.ad_info.district
			};

		}

		public async Task<Address?> GetAddressAsync(Location latlon) {
			latlon = latlon.ConvertToTencentLatLon();

			var url = "https://apis.map.qq.com/ws/geocoder/v1/" + $"?key={_settings.Key}&location={latlon.Lat},{latlon.Lon}";
			var x = await GetApiResultAsync(url);

			return x == null ? null : new Address {
				Location = new Location {
					Lat = x.location.lat,
					Lon = x.location.lng,
					LatLonType = LatLonType.Tencent
				},

				DisplayAddress = x.address,
				DisplayAddressAlternate = x.formatted_addresses?.recommend,

				Province = x.address_component.province,
				City = x.address_component.city,
				District = x.address_component.district,
				Street = x.address_component.street,
				AccuratePlace = x.address_component.street_number
			};
		}

		public async Task<Location?> GetLatLonAsync(string addressName) {
			var url = "https://apis.map.qq.com/ws/geocoder/v1/" + $"?key={_settings.Key}&address={addressName}";
			var x = await GetApiResultAsync(url);

			return x == null ? (Location?)null : new Location {
				Lat = x.location.lat,
				Lon = x.location.lng,
				LatLonType = LatLonType.Tencent
			};
		}

		public async Task<Distance?> GetDistanceAsync(Location from, Location to, DistanceMode mode = DistanceMode.Driving) {
			from = from.ConvertToTencentLatLon();
			to = to.ConvertToTencentLatLon();

			if ( mode == DistanceMode.Straight ) {
				return to.StraightDistanceTo(from);
			}

			var url = "https://apis.map.qq.com/ws/distance/v1/matrix/" +
					  $"?key={_settings.Key}" +
					  $"&mode={mode.ToLower()}" +
					  $"&from={from.Lat},{from.Lon}" +
					  $"&to={to.Lat},{to.Lon}";

			var x = await GetApiResultAsync(url);

			return x == null ? null : new Distance {
				From = from,
				To = to,
				Mode = mode,
				Meters = (int)x.rows[0].elements[0].distance,
				TravelTimeEstimate = TimeSpan.FromSeconds((int?)x.rows[0].elements[0].duration ?? 0),
			};
		}

		public async Task<Distance[]?> GetDistancesAsync(Location from, IEnumerable<Location> toMany, DistanceMode mode = DistanceMode.Driving) {
			from = from.ConvertToTencentLatLon();

			if ( mode == DistanceMode.Straight ) {
				return toMany.Select(to => to.StraightDistanceTo(from)).ToArray();
			}

			var url = "https://apis.map.qq.com/ws/distance/v1/matrix/" +
					  $"?key={_settings.Key}" +
					  $"&mode={mode.ToLower()}" +
					  $"&from={from.Lat},{from.Lon}" +
					  $"&to={string.Join(';', toMany.Select(x => x.ConvertToTencentLatLon().ToString()))}";

			var x = await GetApiResultAsync(url);
			if ( x == null ) {
				return null;
			}

			var n = toMany.Count();
			var i = 0;
			var results = new Distance[n];
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

		private async Task<dynamic?> GetApiResultAsync(string url) {
			try {
				var json = await _httpClient.GetStringAsync(url);
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
