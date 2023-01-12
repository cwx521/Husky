using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Husky.WeChatIntegration
{
	public class WeChatPayCertifiedHttpClientHandler : HttpClientHandler
	{
		internal WeChatPayCertifiedHttpClientHandler(
			string wechatPayMerchantIdAsCertSubjectName,
			StoreName certStoreName = StoreName.My,
			StoreLocation certStoreLocation = StoreLocation.LocalMachine
		) : base() {

			using var store = new X509Store(certStoreName, certStoreLocation);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

			var cert = store.Certificates.Find(X509FindType.FindBySubjectName, wechatPayMerchantIdAsCertSubjectName, false).FirstOrDefault();
			if (cert != null) {
				ClientCertificateOptions = ClientCertificateOption.Manual;
				SslProtocols =  SslProtocols.Tls12;
				ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
				ClientCertificates.Add(cert);
			}
			store.Close();
		}

		internal WeChatPayCertifiedHttpClientHandler(
			string? wechatPayMerchantIdAsCertPassword,
			string certFile
		) : base() {

			var cert = new X509Certificate2(certFile, wechatPayMerchantIdAsCertPassword, X509KeyStorageFlags.MachineKeySet);
			if (cert != null) {
				ClientCertificateOptions = ClientCertificateOption.Manual;
				SslProtocols = SslProtocols.Tls12;
				ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
				ClientCertificates.Add(cert);
			}
		}
	}
}
