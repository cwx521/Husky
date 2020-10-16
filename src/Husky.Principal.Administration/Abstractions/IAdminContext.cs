using System;

namespace Husky.Principal
{
	public interface IAdminContext
	{
		IPrincipalUser Principal { get; }

		int Id { get; }
		string DisplayName { get; }

		bool IsAuthorized { get; }
		string[] Roles { get; }
		long Powers { get; }
		TEnum MapPowers<TEnum>() where TEnum : Enum;
		bool Allow<TEnum>(TEnum power) where TEnum : Enum;

		void Grant(AdminRolePower rolePower);
		void Destroy();
	}
}
