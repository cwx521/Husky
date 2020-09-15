# Husky
contact me at chenwx521#hotmail.com (replace # to @)

For asp.net core 3.1 or later versions.

put these at ConfigureServices in Startup.cs

Example
---------------------

public class Startup
{
	public Startup(IConfiguration configuration) {
		Configuration = configuration;
	}

	public IConfiguration Configuration { get; }

	public void ConfigureServices(IServiceCollection services) {
		//Config
		services.AddSingleton(Configuration);
		Crypto.PermanentToken = Configuration.GetValue<string>("Security:PermanentToken");

		var defaultConnectionString = Configuration.SeekConnectionString<MainDbContext>();

		//Husky
		services.Husky()
			.AddPrincipal(IdentityCarrier.Cookie, new IdentityOptions { SessionMode = true })
			.AddDiagnostics(x => x.UseSqlServer(defaultConnectionString))
			.AddKeyValueManager(x => x.UseSqlServer(defaultConnectionString))
			.AddMailSender(x => x.UseSqlServer(defaultConnectionString))
			.AddTwoFactor(x => x.UseSqlServer(defaultConnectionString))
			.AddAliyunSms(Configuration.GetSection("AliyunSms").Get<AliyunSmsSettings>())
			.AddWeChatIntegration(Configuration.GetSection("WeChat").Get<WeChatAppConfig>())
			;
      
		// .....
    
		//AspNetCore
		services.AddMvc(mvc => {
			mvc.Filters.Add<ExceptionLogHandlerFilter>();
			mvc.Filters.Add<RequestLogHandlerFilter>();
			//.....
		});
    
		//...........
	}
  
	//........
}
