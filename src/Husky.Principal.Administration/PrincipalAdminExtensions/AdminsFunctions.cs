using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Administration
{
	public class AdminsFunctions
	{
		internal AdminsFunctions(IPrincipalAdmin principalAdmin) {
			_admin = principalAdmin;
			_db = principalAdmin.Principal.ServiceProvider.GetRequiredService<IAdminsDbContext>();
		}

		private readonly IPrincipalAdmin _admin;
		private readonly IAdminsDbContext _db;

		public async Task<Result<Admin>> CreateAdminAsync(int associatedUserId, string displayName) {
			var row = await _db.Admins
				.IgnoreQueryFilters()
				.Include(x => x.InRoles)
				.Where(x => x.UserId == associatedUserId)
				.SingleOrDefaultAsync();

			//Create if it does not exist
			if ( row == null ) {
				row = new Admin {
					UserId = associatedUserId,
					DisplayName = displayName
				};
				_db.Admins.Add(row);
			}

			//Bring back if it exists but deleted
			else if ( row.Status == RowStatus.Deleted ) {
				row.InRoles.Clear();
				row.DisplayName = displayName;
				row.Status = RowStatus.Active;
			}

			var validationResult = ValidatorHelper.Validate(row);
			if ( !validationResult.Ok ) {
				return new Failure<Admin>(validationResult.Message);
			}

			await _db.Normalize().SaveChangesAsync();
			return new Success<Admin>(row);
		}

		public async Task<Result> ChangeDisplayNameAsync(Guid adminId, string newDisplayName) {
			var row = await _db.Admins.FindAsync(adminId);
			if ( row == null ) {
				return new Failure($"管理员不存在");
			}

			row.DisplayName = newDisplayName;

			var validationResult = ValidatorHelper.ValidateProperty(row, row => row.DisplayName);
			if ( !validationResult.Ok ) {
				return validationResult;
			}

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> SetAdminRolesAsync(Guid adminId, params int[] adminRoleIdArray) {
			var row = await _db.Admins
				.Include(x => x.InRoles)
				.Where(x => x.Id == adminId)
				.SingleOrDefaultAsync();

			if ( row == null ) {
				return new Failure($"管理员不存在");
			}

			var givingRoleIdArray = adminRoleIdArray.Where(x => !row.InRoles.Any(y => y.RoleId == x));
			var giving = _db.AdminInRoles.Where(x => givingRoleIdArray.Contains(x.RoleId));
			row.InRoles.AddRange(giving);
			row.InRoles.RemoveAll(x => !adminRoleIdArray.Contains(x.RoleId));

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> GrantAdminRoleAsync(Guid adminId, int adminRoleId) {
			if ( !_db.Admins.Any(x => x.Id == adminId) ) {
				return new Failure("管理员账号不存在");
			}

			_db.Normalize().AddOrUpdate(new AdminInRole {
				AdminId = adminId,
				RoleId = adminRoleId
			});
			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> WithdrawAdminRoleAsync(Guid adminId, int adminRoleId) {
			var match = _db.AdminInRoles.Where(x => x.AdminId == adminId && x.RoleId == adminRoleId);
			_db.AdminInRoles.RemoveRange(match);
			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> DeleteAdminAsync(Guid adminId, bool suspendInsteadOfDeleting = false) {
			var row = await _db.Admins.FindAsync(adminId);
			if ( row != null ) {
				if ( row.IsInitiator ) {
					return new Failure("不能删除初始管理员");
				}
				row.Status = suspendInsteadOfDeleting ? RowStatus.Suspended : RowStatus.Deleted;
				await _db.Normalize().SaveChangesAsync();
			}
			return new Success();
		}
	}
}
