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
			_options = new QQLbsOptions {
				Key = key ?? throw new ArgumentNullException(nameof(key))
			};
		}

		public QQLbsService(QQLbsOptions settings) {
			_options = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		private readonly QQLbsOptions _options;

		public async Task<Address?> GetAddressAsync(IPAddress ip) {
			var ipString = ip.MapToIPv4().ToString();
			var url = "https://apis.map.qq.com/ws/location/v1/ip" + $"?key={_options.Key}&ip={ipString}";
			var x = await GetApiResultAsync(url);

			return x == null ? null : new Address {
				Location = new Location {
					Lat = x.location.lat,
					Lon = x.location.lng,
					LatLonType = LatLonType.Gcj02
				},
				Province = x.ad_info.province,
				City = x.ad_info.city,
				District = x.ad_info.district
			};
		}

		public async Task<Address?> GetAddressAsync(Location latlon) {
			latlon = latlon.ConvertToGcj02();

			var url = "https://apis.map.qq.com/ws/geocoder/v1/" + $"?key={_options.Key}&location={latlon}";
			var x = await GetApiResultAsync(url);

			return x == null ? null : new Address {
				Location = new Location {
					Lat = x.location.lat,
					Lon = x.location.lng,
					LatLonType = LatLonType.Gcj02
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
			var url = "https://apis.map.qq.com/ws/geocoder/v1/" + $"?key={_options.Key}&address={addressName}";
			var x = await GetApiResultAsync(url);

			return x == null ? null : new Location {
				Lat = x.location.lat,
				Lon = x.location.lng,
				LatLonType = LatLonType.Gcj02
			};
		}

		public async Task<Distance?> GetDistanceAsync(Location from, Location to, DistanceMode mode = DistanceMode.Driving) {
			from = from.ConvertToGcj02();
			to = to.ConvertToGcj02();

			if (mode == DistanceMode.Straight) {
				return to.StraightDistanceTo(from);
			}

			var url = "https://apis.map.qq.com/ws/distance/v1/matrix/" +
					  $"?key={_options.Key}" +
					  $"&mode={mode.ToLower()}" +
					  $"&from={from}" +
					  $"&to={to}";

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
			from = from.ConvertToGcj02();

			if (mode == DistanceMode.Straight) {
				return toMany.Select(to => to.StraightDistanceTo(from)).ToArray();
			}

			var url = "https://apis.map.qq.com/ws/distance/v1/matrix/" +
					  $"?key={_options.Key}" +
					  $"&mode={mode.ToLower()}" +
					  $"&from={from}" +
					  $"&to={string.Join(';', toMany.Select(x => x.ConvertToGcj02().ToString()))}";

			var x = await GetApiResultAsync(url);
			if (x == null) {
				return null;
			}

			var n = toMany.Count();
			var i = 0;
			var results = new Distance[n];
			foreach (var to in toMany) {
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

		private static async Task<dynamic?> GetApiResultAsync(string url) {
			try {
				var json = await HttpClientSingleton.Instance.GetStringAsync(url);
				var d = JsonConvert.DeserializeObject<dynamic>(json)!;

				if (d.status == 0) {
					return d.result;
				}
			}
			catch { }

			return null;
		}
	}
}
