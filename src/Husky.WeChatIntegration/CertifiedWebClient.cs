using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Husky.WeChatIntegration
{
	public class CertifiedWebClient : WebClient
	{
		internal CertifiedWebClient(string findValue) : base() {
			_findValue = findValue;
		}

		private readonly string _findValue;

		protected override WebRequest GetWebRequest(Uri address) {
			var request = (HttpWebRequest)base.GetWebRequest(address);

			using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

			var certs = store.Certificates.Find(X509FindType.FindBySubjectName, _findValue, false);
			if ( certs != null && certs.Count != 0 ) {
				request.ClientCertificates.Add(certs[0]);
			}
			store.Close();

			return request;
		}
	}
}
