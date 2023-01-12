using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Husky.WeChatIntegration
{
	public class CertifiedWxpayHttpClientHandler : HttpClientHandler
	{
		private X509Certificate2? _cert;

		internal CertifiedWxpayHttpClientHandler(
			string? wxpayMerchantIdAsPassword,
			string certFile
		) : base() {

			_cert = new X509Certificate2(certFile, wxpayMerchantIdAsPassword, X509KeyStorageFlags.MachineKeySet);
			if (_cert != null) {
				SetCertificatePropertyValues();
			}
		}

		internal CertifiedWxpayHttpClientHandler(
			string wxpayMerchantIdAsCertSubjectName,
			StoreName certStoreName = StoreName.My,
			StoreLocation certStoreLocation = StoreLocation.LocalMachine
		) : base() {

			using var store = new X509Store(certStoreName, certStoreLocation);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

			_cert = store.Certificates.Find(X509FindType.FindBySubjectName, wxpayMerchantIdAsCertSubjectName, false).FirstOrDefault();
			if (_cert != null) {
				SetCertificatePropertyValues();
			}
			store.Close();
		}

		private void SetCertificatePropertyValues() {
			ClientCertificateOptions = ClientCertificateOption.Manual;
			SslProtocols = SslProtocols.Tls12;
			ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
			ClientCertificates.Add(_cert!);
		}

		protected override void Dispose(bool disposing) {
			if (disposing && _cert != null) {
				_cert.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
