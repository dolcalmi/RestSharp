using System;
#if NETFX_CORE
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#else
using System.Security.Cryptography;
#endif
using System.Text;


namespace RestSharp.Authenticators.OAuth.Extensions
{
	internal static class OAuthExtensions
	{
		public static string ToRequestValue(this OAuthSignatureMethod signatureMethod)
		{
			var value = signatureMethod.ToString().ToUpper();
			var shaIndex = value.IndexOf("SHA1");
			return shaIndex > -1 ? value.Insert(shaIndex, "-") : value;
		}

		public static OAuthSignatureMethod FromRequestValue(this string signatureMethod)
		{
			switch (signatureMethod)
			{
				case "HMAC-SHA1":
					return OAuthSignatureMethod.HmacSha1;
				case "RSA-SHA1":
					return OAuthSignatureMethod.RsaSha1;
				default:
					return OAuthSignatureMethod.PlainText;
			}
		}

#if NETFX_CORE
        public static string HashWith(this string input, byte[] keyBytes, MacAlgorithmProvider algorithm)
		{
            var messageBytes = Encoding.UTF8.GetBytes(input);
            MacAlgorithmProvider objMacProv = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA1");
            CryptographicKey hmacKey = algorithm.CreateKey(keyBytes.AsBuffer());
            IBuffer buffHMAC = CryptographicEngine.Sign(hmacKey, messageBytes.AsBuffer());
            return CryptographicBuffer.EncodeToBase64String(buffHMAC);
		}

#else
		public static string HashWith(this string input, HashAlgorithm algorithm)
		{
			var data = Encoding.UTF8.GetBytes(input);
			var hash = algorithm.ComputeHash(data);
			return Convert.ToBase64String(hash);
		}
#endif


	}
}