using System;
using Husky.Principal.SessionData;

namespace Husky.Principal
{
	public class AdminContext : IAdminContext
	{
		public AdminContext(IPrincipalUser principal) {
			Principal = principal;
		}

		private const string _key = nameof(AdminRolePower);

		public IPrincipalUser Principal { get; }

		public int Id => IsAuthorized ? Principal.Id : 0;
		public string DisplayName => Principal.DisplayName;

		public bool IsAuthorized => _rowPower != null;
		public long Powers => _rowPower?.Powers ?? 0;
		public string[] Roles => _rowPower?.Roles ?? new string[0];

		public TEnum MapPowers<TEnum>() where TEnum : Enum => (TEnum)(object)Powers;
		public bool Allow<TEnum>(TEnum power) where TEnum : Enum => MapPowers<TEnum>().HasFlag(power);


		public void Grant(AdminRolePower rolePower) {
			if ( Principal.IsAnonymous || !(Principal.SessionData() is SessionDataContainer sessionData) ) {
				throw new InvalidProgramException(
					"AdminContext depends on authenticated Principal, " +
					"this method can not be called when Principal is anonymous."
				);
			}
			_rowPower = rolePower;
			sessionData.AddOrUpdate(_key, _rowPower, (ket, data) => _rowPower);
		}

		public void Destroy() => Principal.SessionData()?.TryRemove(_key, out _);


		private AdminRolePower? _rowPower;
		private AdminRolePower? RolePower {
			get {
				if ( _rowPower == null &&
					 Principal.IsAuthenticated &&
					 Principal.SessionData() is SessionDataContainer sessionData ) {

					if ( sessionData.TryGetValue(_key, out var found) ) {
						_rowPower = (AdminRolePower)found;
					}
				}
				return _rowPower;
			}
		}
	}
}
