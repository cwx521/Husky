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

		TFlaggedEnum MatchPowers<TFlaggedEnum>() where TFlaggedEnum : Enum;
		bool Allow<TFlaggedEnum>(TFlaggedEnum power) where TFlaggedEnum : Enum;
	}
}
