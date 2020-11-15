using System;
using System.Diagnostics.CodeAnalysis;
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

		public long PowerFlags => _adminData?.Powers ?? 0;
		public string[] Roles => _adminData?.Roles ?? Array.Empty<string>();

		public TEnum Powers<TEnum>() where TEnum : Enum => (TEnum)(object)PowerFlags;

		[SuppressMessage("Usage", "CA2248:Provide correct 'enum' argument to 'Enum.HasFlag'")]
		public bool Allow<TEnum>(TEnum power) where TEnum : Enum => Powers<TEnum>().HasFlag(power);


		public const string AdminDataKey = nameof(AdminViewModel);

		protected virtual AdminViewModel? InitAdminData() {
			if ( Principal.IsAnonymous ) {
				return null;
			}

			return (AdminViewModel?)Principal.Cache().GetOrAdd(AdminDataKey, key => {
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
					Powers = admin?.InRoles.Aggregate(0L, (i, x) => i | x.Role.Powers)
				};
			});
		}
	}
}
