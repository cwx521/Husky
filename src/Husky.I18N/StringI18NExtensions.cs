/// <summary>
/// For now we do nothing, just wrap the strings, so all translations will be getting through this.
/// </summary>
public static class StringI18NExtensions
{
	public static string Xslate(this string str) => str;
	public static string Xslate(this string str, params object[] args) => string.Format(str, args);
}
