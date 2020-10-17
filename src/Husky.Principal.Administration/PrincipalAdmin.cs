using System;
using System.Linq;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Administration
{
	public class PrincipalAdmin : IPrincipalAdmin
	{
		public PrincipalAdmin(IPrincipalUser principal) {
			Principal = principal;
			_adminData = InitAdminData();
		}

		private readonly AdminViewModel? _adminData;

		public IPrincipalUser Principal { get; }

		public Guid Id => _adminData?.Id ?? Guid.Empty;
		public string DisplayName => _adminData?.DisplayName ?? Principal.DisplayName;

		public bool IsAdmin => _adminData != null && _adminData.Id != Guid.Empty;
		public bool IsNotAdmin => !IsAdmin;

		public long Powers => _adminData?.Powers ?? 0;
		public string[] Roles => _adminData?.Roles ?? new string[0];

		public TEnum MapPowers<TEnum>() where TEnum : Enum => (TEnum)(object)Powers;
		public bool Allow<TEnum>(TEnum power) where TEnum : Enum => MapPowers<TEnum>().HasFlag(power);


		public const string AdminDataKey = "AdminData";

		protected virtual AdminViewModel? InitAdminData() {
			if ( Principal.IsAnonymous ) {
				return null;
			}

			return (AdminViewModel?)Principal.SessionData().GetOrAdd(AdminDataKey, key => {
				using var scope = Principal.ServiceProvider.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<IAdminsDbContext>();

				var admin = db.Admins
					.AsNoTracking()
					.Include(x => x.InRoles).ThenInclude(x => x.Role)
					.Where(x => x.UserId == Principal.Id)
					.Where(x => x.Status == RowStatus.Active)
					.SingleOrDefault();

				return new AdminViewModel {
					Id = admin?.Id,
					DisplayName = admin?.DisplayName,
					Roles = admin?.InRoles.Select(x => x.Role.RoleName).ToArray(),
					Powers = admin?.InRoles.Aggregate((long)0, (i, x) => i | x.Role.Powers)
				};
			});
		}
	}
}
