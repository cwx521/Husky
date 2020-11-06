using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Husky.Alipay.Tests
{
	[TestClass()]
	public class AlipayServiceHelperTests
	{
		//attention: fill the required values to run this test

		private readonly AlipayOptions _alipayOptions = new AlipayOptions {
			PartnerId = "2088631202305880",
			AppId = "2019091167225651",
			PrivateKey = "MIIEpAIBAAKCAQEAwNdFGPYoyB6kh9PeLCBFvSfbGV2iGu6sOnY31nCLlwJC+QaETK+NqjiANpKS5luZM/im1Yo7Rkw9qEP/9ZTQ4vD/MTGMNh+q3ZGZDumPfSAkFmpYqa1R4wdS9CStZdnxbC7gQTGRRLEEKZArhbbIPPs78GRuWEDpLU5oLU2X63bDMSMWciPbJOcHWpycCo8H7nHa+/w1lZgYTLfVbCfCeTsZ+j+7cRPWlgwrTvtLNyPjLdn5EsVSNeuqin9iC7/AGbjJ/wdLEUAqNFAkHfwq8w3ltpQ1l139WOiBkFFLTrA6AocVPrYhMuxaFP9EbQY3KhPs4ubpUgByq81ScSteXQIDAQABAoIBAEDns6AFKhDBD4uP6dVcP1y9D2doNmluL5W1uHF6i+EVp8j9QoY1CC9kuPKlPsA90EDHzg9NaUnt20rJKFtV7UCU8K90B8cnvXC50pRMVDk/GiZZQOJLLaaGDouR1LLVOga5eE05C7XdBpOerKdir96miLQOeX1Gy7SuES4+UYW0a3Ol7CGvN54u+/90vV5Qng1yuRjOp74zl3R0kqaeMCEYF0TOqQ9hOxOvubjf5d4LDveMPWNGNB08XgTlCupls1waqxzsw5j7XtResSZi8rWVHpqfUae7wWMuP9CROXzkBGWto0WGUnsTvwYCnJ6H69MBZaF8kmlfnZSMpEDDRwECgYEA7YXiOuidFy1YC5k0lH8OeaibXmrCIbNqC4UrhYcbn5H4/pXo+W0yf97M2CntVNy8EMwElL0/GJnPOcQ6xJD0ERUROcyPTFgLgkIl1t0vUBqHpEySmTnewzVAtk6M0l84fJ3XLg8X/4naATF0GPdn2QkX0ylaSdJ71hxles5i3D0CgYEAz9eSSUM/3MOOxby1hfFU0ipwcT7aHWPqt52mpPN/muRgoD6NJN9jaS55xz3rIuuDaJmkrusaq336GJo5MoEULRUQlNIqXpz+EJA31vRi/1/lZb496UC4tYVbZKz4L+Mr3pzP7IjNFp5kSGNloSDvyiUCaBmStKT7Kj3QhFwcDKECgYEA6EK0HvCZtDFF2Eme7dp0eCiUjTYN0VCU0OEO1spwC5B9D83dfBH4gKuIWZReHE6gpDgr/poz2cWFqrIk2VP5Jb8ZEhDiyz+JOX2fbMTq4AShQNsTuzuohfBL8DlNYOV14f2ijcp6qAnPoL0QDt3WsZc/sd/NB0UIjS1FWln4t+0CgYAU8mf25EUyba8c9Kjix/DBga91al03KxPDz6q04ymQJcYrQEKfXbg6KInzb6UcDtpel2WGj4FPmVG0Ww66rVGoPYA/T3Mft/jPGInYKyczD6oh9+trb69t8/PDEL+uq9mHBQzbjOoGho4a0ZGzjIs+Nz6n2TP06s+5cSNrw5C8IQKBgQDcJ5zIuurTN9e7+P60IuUeWK4bv5y24fG4e7/k4M1EnngFEyVmSTiIALkzak1hcxWy6QuuEPNtJZA8gyJtaK12aDlTQ8nD9a3DVgwj5qZ0zdEGKTqTVI0c5jGEG5620YNeiCgLKuHM3CnDbae7Mw9H4XdSre1CI16avaD/5HW4tg==",
			AlipayPublicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAslSMwqfQx3l5jMueBso4LNKH7wWSQdgRUTWalkMU3kpJPCIsSAfvvS22sIbCWJ64FOGfX9QOPkP7r1Ouvz11hWYaAigokfUD3sHHhw7sF8u63bU4zUhr7GdYh1CpP/B0x2MvZS2UdGGozJE3maYyPyTKVFN8tjltnjR3btzF9RPwIbT0WcyCYg5ZxreuUWUwrFCirPsUprIMF+igYvahPviV+aHPKiZWuI7+T9/qlbKcBMiHYJ1LoIg50EEZLxUtvAkFJ1wRPeEG8NqQTIz63TYGZdvjHFYZTqRVtRH1N1pzK5aMNNIBYyGG4dBCEPf2f33e4daiT23HMB+RZKZyGQIDAQAB",
		};

		[TestMethod()]
		public void GenerateAlipayPaymentUrlTest() {
			if ( string.IsNullOrEmpty(_alipayOptions.PrivateKey) || string.IsNullOrEmpty(_alipayOptions.AlipayPublicKey) ) {
				return;
			}

			var alipay = new AlipayService(_alipayOptions);

			var tradeModel = new AlipayOrderModel {
				Amount = 0.1m,
				OrderNo = OrderIdGen.New(),
				Subject = "UnitTest",
				NotifyUrl = "",
				CallbackUrl = "",
			};
			var paymentUrl = alipay.GenerateAlipayPaymentUrl(tradeModel).DesktopPagePaymentUrl;

			//Payment url is opened up in the default browser
			Process.Start(new ProcessStartInfo(paymentUrl) { UseShellExecute = true });

			//!!!!! THIS TEST REQUIRES MANUAL OPERATION, USE DEBUG MODE AND SET BREAK POINT HERE !!!!
			//Open the url in browser
			//Pay manually in the opened page, then continue
			var queryResult = alipay.QueryOrder(tradeModel.OrderNo);
			Assert.AreEqual(tradeModel.Amount, queryResult.Data.Amount);

			var remainedAmount = tradeModel.Amount;

			//Refund 0.01
			var refundAmount = 0.01m;
			var refundRequestNo = OrderIdGen.New();
			var refundResult = alipay.Refund(tradeModel.OrderNo, refundRequestNo, refundAmount, "Test");
			Assert.IsTrue(refundResult.Ok);

			remainedAmount -= refundAmount;

			//Query refund 0.01
			var queryRefundResult = alipay.QueryRefund(tradeModel.OrderNo, refundRequestNo);
			Assert.AreEqual(refundAmount, queryRefundResult.Data.RefundAmount);

			//Refund another 0.01
			var refundAmount2 = 0.01m;
			var refundRequestNo2 = OrderIdGen.New();
			var refundResult2 = alipay.Refund(tradeModel.OrderNo, refundRequestNo2, refundAmount2, "Test");
			Assert.IsTrue(refundResult2.Ok);

			remainedAmount -= refundAmount2;

			//Query another refund 0.01
			var queryRefundResult2 = alipay.QueryRefund(tradeModel.OrderNo, refundRequestNo2);
			Assert.AreEqual(refundAmount2, queryRefundResult2.Data.RefundAmount);

			//Expected failure refund
			var refundAmount3 = tradeModel.Amount;
			var refundResult3 = alipay.Refund(tradeModel.OrderNo, OrderIdGen.New(), refundAmount3, "Test");
			Assert.IsFalse(refundResult3.Ok);

			//Refund all remained amount
			var refundResult4 = alipay.Refund(tradeModel.OrderNo, OrderIdGen.New(), remainedAmount, "Test");
			Assert.IsTrue(refundResult4.Ok);
		}
	}
}