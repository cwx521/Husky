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
			SslProtocols ssl = SslProtocols.Tls12,
			StoreName certStoreName = StoreName.My,
			StoreLocation certStoreLocation = StoreLocation.LocalMachine
		) : base() {

			using var store = new X509Store(certStoreName, certStoreLocation);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

			var certs = store.Certificates.Find(X509FindType.FindBySubjectName, wechatPayMerchantIdAsCertSubjectName, false);
			if (certs != null && certs.Count != 0) {

				ClientCertificateOptions = ClientCertificateOption.Manual;
				SslProtocols = ssl;
				ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
				ClientCertificates.Add(certs.First());
			}
			store.Close();
		}
	}
}
