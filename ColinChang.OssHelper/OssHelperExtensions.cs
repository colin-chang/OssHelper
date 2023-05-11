using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Aliyun.OSS;

namespace ColinChang.OssHelper
{
    public static class OssHelperExtensions
    {
        public static IServiceCollection AddOssHelper(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services.AddHttpClient();

            services.AddOptions<OssHelperOptions>()
                .Configure(configuration.Bind)
                .ValidateDataAnnotations();
            services.AddSingleton<IOptionsChangeTokenSource<OssHelperOptions>>(
                new ConfigurationChangeTokenSource<OssHelperOptions>(configuration));
            services.AddSingleton<IOssHelper, OssHelper>();
            return services;
        }

        public static IServiceCollection AddOssHelper(this IServiceCollection services,
            Action<OssHelperOptions> configureOptions)
        {
            if (services == null)
                throw new ArgumentException(nameof(services));

            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);
            services.AddSingleton<IOssHelper, OssHelper>();
            return services;
        }

        /// <summary>
        /// 验证 OSS 签名
        /// </summary>
        public static async Task<bool> VerifyOssSignatureAsync(this HttpRequest request,
            IEnumerable<string> publicKeyIssuers, HttpClient httpClient)
        {
            //authorization
            var authorization = request.Headers["authorization"].ToString() ??
                                request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authorization))
                throw new OssException("authorization property in the http request header is null");
            var authorizationBytes = Convert.FromBase64String(authorization);

            //public key
            var url = Encoding.ASCII.GetString(
                Convert.FromBase64String(request.Headers["x-oss-pub-key-url"].ToString()));
            if (!publicKeyIssuers.Any(issuer => issuer.Contains(new Uri(url, UriKind.Absolute).Host)))
                throw new OssException("invalid key issuer");
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) => true;
            using var pemRequest = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await httpClient.SendAsync(pemRequest);
            response.EnsureSuccessStatusCode();
            var publicKey = await response.Content.ReadAsStringAsync();
            publicKey = publicKey
                .Replace("-----BEGIN PUBLIC KEY-----\n", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "");
            var publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            var publicKeyXml =
                $"<RSAKeyValue><Modulus>{Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned())}</Modulus><Exponent>{Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned())}</Exponent></RSAKeyValue>";

            //Generate the New Authorization string according to the HttpRequest
            // 特殊字符UrlEncode后使用小写，OSS验证要求使用大写，两者MD5不同会导致验证签名失败
            var builder = new StringBuilder();
            foreach (var key in request.Form.Keys)
            {
                builder.Append(key).Append("=");
                var val = request.Form[key];
                foreach (var c in val.ToString())
                {
                    if (char.IsLetterOrDigit(c))
                        builder.Append(c);
                    else
                        builder.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }

                builder.Append("&");
            }

            builder.Remove(builder.Length - 1, 1);
            var callbackBody = builder.ToString();

            string authSource;
            var prefix = request.Headers.ContainsKey("X-Forwarded-Prefix")
                ? $"/{request.Headers["X-Forwarded-Prefix"]}"
                : string.Empty;
            if (request.QueryString.HasValue)
            {
                var urlParts = request.Path.Value.Split('?');
                authSource = $"{prefix}{HttpUtility.UrlDecode(urlParts[0])}?{urlParts[1]}\n{callbackBody}";
            }
            else
                authSource = $"{prefix}{HttpUtility.UrlDecode(request.Path.Value)}\n{callbackBody}";

            using var md5 = MD5.Create();
            var authMd5 = md5.ComputeHash(Encoding.UTF8.GetBytes(authSource));

            var rsa = new RSACryptoServiceProvider();
            try
            {
                rsa.FromXmlString(publicKeyXml);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException(
                    $"VerifySignature Failed : RSADeformatter.VerifySignature get null argument : {e} .");
            }
            catch (CryptographicException e)
            {
                throw new CryptographicException($"VerifySignature Failed : RSA.FromXmlString Exception : {e} .");
            }

            var rsaDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
            rsaDeformatter.SetHashAlgorithm("MD5");

            bool result;
            try
            {
                result = rsaDeformatter.VerifySignature(authMd5, authorizationBytes);
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException(
                    $"VerifySignature Failed : RSADeformatter.VerifySignature get null argument : {e} .");
            }
            catch (CryptographicUnexpectedOperationException e)
            {
                throw new CryptographicUnexpectedOperationException(
                    $"VerifySignature Failed : RSADeformatter.VerifySignature Exception : {e} .");
            }

            return result;
        }

        /// <summary>
        /// 解析回调对象
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static OssObject ExtractOssObject(this HttpRequest request)
        {
            var obj = new OssObject();

            if (request.Form.ContainsKey(nameof(obj.Bucket).ToLower()))
                obj.Bucket = request.Form[nameof(obj.Bucket).ToLower()].ToString();
            if (request.Form.ContainsKey(nameof(obj.Object).ToLower()))
                obj.Object = request.Form[nameof(obj.Object).ToLower()].ToString();

            string filename;
            if (request.Form.ContainsKey(nameof(filename)))
                obj.Object = request.Form[nameof(filename)].ToString();
            if (request.Form.ContainsKey(nameof(obj.Size).ToLower()))
                obj.Size = Convert.ToInt64(request.Form[nameof(obj.Size).ToLower()].ToString());
            if (request.Form.ContainsKey(nameof(obj.MimeType).ToLower()))
                obj.MimeType = request.Form[nameof(obj.MimeType).ToLower()].ToString();

            return obj;
        }

        /// <summary>
        /// 校验回调对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="ossClient"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="OssException"></exception>
        public static void Verify(this OssObject obj, IOss ossClient, ObjectOption options)
        {
            if (string.IsNullOrWhiteSpace(obj.Object))
                throw new OssException($"{nameof(obj.Object).ToLower()} is required");
            if (obj.Size > options.MaxSize)
            {
                ossClient.DeleteObject(obj.Bucket, obj.Object);
                throw new OssException($"{obj.Object} is oversize");
            }

            if (options.AllowedMimeTypes.Contains(obj.MimeType)) return;
            ossClient.DeleteObject(obj.Bucket, obj.Object);
            throw new OssException("unsupported file format");
        }
    }
}