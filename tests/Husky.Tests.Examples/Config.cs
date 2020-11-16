using Husky.KeyValues;
using Microsoft.Extensions.Configuration;

namespace Husky.Tests.Examples
{
	public class Config : ConfigBase
	{
		public Config(IKeyValueManager keyValues, IConfiguration? appSettings) : base(keyValues, appSettings) {
		}

		public string JustConfigureYourSettingsInThisWay { get; set; } = "ConfiguredValue";
	}
}
