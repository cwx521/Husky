namespace Husky.Principal.Administration
{
	public static class PrincipalAdminExtensions
	{
		public static AdminsAdminFunctions Admins(this IPrincipalAdmin admin) => new AdminsAdminFunctions(admin);
		public static AdminRolesAdminFunctions AdminRoles(this IPrincipalAdmin admin) => new AdminRolesAdminFunctions(admin);
	}
}
