using System;

namespace Husky.Principal.Administration
{
	public interface IPrincipalAdmin
	{
		IPrincipalUser Principal { get; }

		bool IsAdmin { get; }
		bool IsNotAdmin { get; }

		Guid Id { get; }
		string DisplayName { get; }

		string[] Roles { get; }
		long Powers { get; }

		TEnum MapPowers<TEnum>() where TEnum : Enum;
		bool Allow<TEnum>(TEnum power) where TEnum : Enum;
	}
}
