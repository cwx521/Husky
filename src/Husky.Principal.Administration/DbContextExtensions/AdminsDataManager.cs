using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Administration
{
	public static class AdminsDataManager
	{
		public static async Task<Result<Admin>> CreateAdmin(this IAdminsDbContext db, int associatedUserId, string displayName) {
			var row = db.Admins
				.Include(x => x.InRoles)
				.Where(x => x.UserId == associatedUserId)
				.SingleOrDefault();

			//Create if it does not exist
			if ( row == null ) {
				row = new Admin {
					UserId = associatedUserId,
					DisplayName = displayName
				};
				db.Admins.Add(row);
			}

			//Bring back if it exists but deleted
			else if ( row.Status == RowStatus.Deleted ) {
				row.InRoles.Clear();
				row.DisplayName = displayName;
				row.Status = RowStatus.Active;
			}

			var validationResult = ValidatorHelper.Validate(row);
			if ( !validationResult.Ok ) {
				return validationResult;
			}

			await db.Normalize().SaveChangesAsync();
			return new Success<Admin>(row);
		}

		public static async Task<Result> ChangeDisplayName(this IAdminsDbContext db, Guid adminId, string newDisplayName) {
			var row = db.Admins.Find(adminId);
			if ( row == null ) {
				return new Failure($"管理员不存在");
			}

			row.DisplayName = newDisplayName;

			var validationResult = ValidatorHelper.Validate(row, row => row.DisplayName);
			if ( !validationResult.Ok ) {
				return validationResult;
			}

			await db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public static async Task<Result> SetAdminRoles(this IAdminsDbContext db, Guid adminId, params int[] adminRoleIdArray) {
			var row = db.Admins
				.Include(x => x.InRoles)
				.Where(x => x.Id == adminId)
				.SingleOrDefault();

			if ( row == null ) {
				return new Failure($"管理员不存在");
			}

			var givingRoleIdArray = adminRoleIdArray.Where(x => !row.InRoles.Any(y => y.RoleId == x));
			var giving = db.AdminInRoles.Where(x => givingRoleIdArray.Contains(x.RoleId));
			row.InRoles.AddRange(giving);
			row.InRoles.RemoveAll(x => !adminRoleIdArray.Contains(x.RoleId));

			await db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public static async Task<Result> GrantAdminRole(this IAdminsDbContext db, Guid adminId, int adminRoleId) {
			if ( !db.Admins.Any(x => x.Id == adminId) ) {
				return new Failure("管理员用户不存在");
			}

			db.Normalize().AddOrUpdate(new AdminInRole {
				AdminId = adminId,
				RoleId = adminRoleId
			});
			await db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public static async Task<Result> WithdrawAdminRole(this IAdminsDbContext db, Guid adminId, int adminRoleId) {
			var row = db.AdminInRoles.SingleOrDefault(x => x.AdminId == adminId && x.RoleId == adminRoleId);
			if ( row != null ) {
				db.AdminInRoles.Remove(row);
				await db.Normalize().SaveChangesAsync();
			}
			return new Success();
		}

		public static async Task<Result> DeleteAdmin(this IAdminsDbContext db, Guid adminId, bool suspendInsteadOfDeleting = false) {
			var row = db.Admins.Find(adminId);
			if ( row != null ) {
				row.Status = suspendInsteadOfDeleting ? RowStatus.Suspended : RowStatus.Deleted;
				await db.Normalize().SaveChangesAsync();
			}
			return new Success();
		}
	}
}
