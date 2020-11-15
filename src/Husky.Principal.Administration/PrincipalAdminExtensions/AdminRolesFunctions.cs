using System;
using System.Linq;
using System.Threading.Tasks;
using Husky.Principal.Administration.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Husky.Principal.Administration
{
	public class AdminRolesFunctions
	{
		internal AdminRolesFunctions(IPrincipalAdmin principalAdmin) {
			_admin = principalAdmin;
			_db = principalAdmin.Principal.ServiceProvider.GetRequiredService<IAdminsDbContext>();
		}

		private readonly IPrincipalAdmin _admin;
		private readonly IAdminsDbContext _db;

		public async Task<Result<AdminRole>> CreateRoleAsync(string roleName, long powers) {
			if ( _db.AdminRoles.Any(x => x.RoleName == roleName) ) {
				return new Failure<AdminRole>($"管理组名称“{roleName}”已存在");
			}

			var adminRole = new AdminRole {
				RoleName = roleName,
				Powers = powers
			};
			_db.AdminRoles.Add(adminRole);

			var validationResult = ValidatorHelper.Validate(adminRole);
			if ( !validationResult.Ok ) {
				return new Failure<AdminRole>(validationResult.Message);
			}

			await _db.Normalize().SaveChangesAsync();
			return new Success<AdminRole>(adminRole);
		}

		public async Task<Result> DeleteRoleAsync(int roleId) {
			var adminRole = await _db.AdminRoles.FindAsync(roleId);
			if ( adminRole != null ) {
				_db.AdminRoles.Remove(adminRole);
			}
			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> ChangeRolePropValueAsync<T>(int roleId, string adminRolePropertyName, T propertyValue) {
			var allowedPropertyNames = new[] {
				nameof(AdminRole.RoleName),
				nameof(AdminRole.Powers)
			};
			if ( !allowedPropertyNames.Contains(adminRolePropertyName) ) {
				return new Failure("不允许修改该字段内容");
			}

			var adminRole = await _db.AdminRoles.FindAsync(roleId);
			if ( adminRole == null ) {
				return new Failure($"管理组#{roleId}不存在");
			}

			typeof(AdminRole).GetProperty(adminRolePropertyName)!.SetValue(adminRole, propertyValue);

			var validationResult = ValidatorHelper.ValidateProperty(adminRole, QueryableHelper.Select<AdminRole, T>(adminRolePropertyName));
			if ( !validationResult.Ok ) {
				return validationResult;
			}

			await _db.Normalize().SaveChangesAsync();
			return new Success();
		}

		public async Task<Result> ChangeRoleNameAsync(int roleId, string roleName) => await ChangeRolePropValueAsync(roleId, nameof(AdminRole.RoleName), roleName);
		public async Task<Result> ChangeRolePowersAssignmentAsync(int roleId, long powers) => await ChangeRolePropValueAsync(roleId, nameof(AdminRole.Powers), powers);
	}
}
