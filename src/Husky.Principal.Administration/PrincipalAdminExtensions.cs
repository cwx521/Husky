namespace Husky.Principal.Administration
{
	public static class PrincipalAdminExtensions
	{
		public static AdminsFunctions Admins(this IPrincipalAdmin admin) => new AdminsFunctions(admin);
		public static AdminRolesFunctions AdminRoles(this IPrincipalAdmin admin) => new AdminRolesFunctions(admin);
	}
}
