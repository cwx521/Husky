namespace Husky.Sugar
{
	public class AppVariables
    {
		public const string SectionName = nameof(AppVariables);

		public string SiteName { get; set; }
		public string SiteUrl { get; set; }

		public string PermanentToken { get; set; }
		public string RerollableToken { get; set; }
		public string DynamicToken => Crypto.RandomString();
	}
}
