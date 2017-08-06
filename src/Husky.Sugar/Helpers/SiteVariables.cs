namespace Husky.Sugar
{
	public class SiteVariables
    {
		public const string ConfigurationSectionName = "Site";

		public string SiteName { get; set; }
		public string SiteUrl { get; set; }
		public string SecretToken { get; set; }
    }
}
