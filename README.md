# Husky
contact me at chenwx521#hotmail.com (replace # to @)

For asp.net core 3.1 or later versions.


Components
---------------------

Husky.Alipay -- 简化封装支付宝接口常用方法，方便编码，快速使用

Husky.AliyunSms -- 封装阿里云手机短信服务接口方法

Husky.DataAudit -- 数据库自动建立Audit更新记录 （该项目未充分测试）

Husky.Diagnostics -- 自动将程序异常和URL访问记录到指定的数据库

Husky.GridQuery -- 快速适配kendoGrid，需要脚本（二次封装脚本稍晚提供）

Husky.Helper -- 常用便捷方法及常用模型对象

Husky.Html -- 快速输出HTML（适配bootstrap4）（该项目未充分测试）

Husky.KeyValues -- 快速建立Key/Values配置表到指定数据库（更具体使用演示稍晚提供）

Husky.Mail -- 简化封装发送邮件功能

Husky.Principal -- 当前用户身份信息保存和验证

Husky.Principal.SessionData -- 实现当前用户会话数据缓存功能

Husky.TwoFactor -- 简化实现短消息二次验证

Husky.WeChatIntegration -- 简化微信API访问请求


Example
---------------------

//put these at ConfigureServices in Startup.cs

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
    
		//.....
	}
  
	//.....
}
