using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;

namespace Husky.Principal.Administration
{
	public static class AdminRolesDataManager
	{
		public static async Task<Result<AdminRole>> CreateRole(this IAdminsDbContext db, string roleName, long powers) {
			if ( db.AdminRoles.Any(x => x.RoleName == roleName) ) {
				return new Failure<AdminRole>($"管理组名称“{roleName}”已存在");
			}

			var adminRole = new AdminRole {
				RoleName = roleName,
				Powers = powers
			};
			db.AdminRoles.Add(adminRole);

			var validationResult = ValidatorHelper.Validate(adminRole);
			if ( !validationResult.Ok ) {
				return validationResult;
			}

			await db.Normalize().SaveChangesAsync();
			return new Success<AdminRole>(adminRole);
		}

		public static async Task<Result> DeleteRole(this IAdminsDbContext db, int roleId) {
			var adminRole = db.AdminRoles.Find(roleId);
			if ( adminRole != null ) {
				db.AdminRoles.Remove(adminRole);
			}
			await db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public static async Task<Result> ChangeRolePropValue<T>(this IAdminsDbContext db, int roleId, string adminRolePropertyName, T propertyValue) {
			var allowedPropertyNames = new[] {
				nameof(AdminRole.RoleName),
				nameof(AdminRole.Powers)
			};
			if ( !allowedPropertyNames.Contains(adminRolePropertyName) ) {
				return new Failure("不允许修改该字段内容");
			}

			var adminRole = db.AdminRoles.Find(roleId);
			if ( adminRole == null ) {
				return new Failure($"管理组#{roleId}不存在");
			}

			typeof(AdminRole).GetProperty(adminRolePropertyName)!.SetValue(adminRole, propertyValue);

			var validationResult = ValidatorHelper.Validate(adminRole, QueryableExtensions.SelectProperty<AdminRole, T>(adminRolePropertyName));
			if ( !validationResult.Ok ) {
				return validationResult;
			}

			await db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public static async Task<Result> ChangeRoleName(this IAdminsDbContext db, int roleId, string roleName) => await db.ChangeRolePropValue(roleId, nameof(AdminRole.RoleName), roleName);
		public static async Task<Result> ChangeRolePowersAssignment(this IAdminsDbContext db, int roleId, long powers) => await db.ChangeRolePropValue(roleId, nameof(AdminRole.Powers), powers);
	}
}
