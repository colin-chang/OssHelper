using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Auth.Sts;
using Aliyun.Acs.Core.Profile;
using Aliyun.OSS;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace ColinChang.OssHelper
{
    public class OssHelper : IOssHelper
    {
        private readonly OssHelperOptions _options;
        private readonly IOss _oss;
        public HttpClient HttpClient { get; set; }

        public OssHelper(IOptionsMonitor<OssHelperOptions> options, HttpClient httpClient) :
            this(options.CurrentValue) =>
            HttpClient = httpClient;

        public OssHelper(OssHelperOptions options)
        {
            _options = options;
            _oss = new OssClient(_options.PolicyOptions.EndPoint, _options.PolicyOptions.AccessKeyId,
                _options.PolicyOptions.AccessKeySecret);
        }

        public async Task<AssumeRoleResponse.AssumeRole_Credentials> GetStsAsync()
        {
            return await Task.Run(() =>
            {
                var profile = DefaultProfile.GetProfile(_options.StsOptions.RegionId, _options.StsOptions.AccessKeyId,
                    _options.StsOptions.AccessKeySecret);
                profile.AddEndpoint(_options.StsOptions.RegionId, _options.StsOptions.RegionId,
                    _options.StsOptions.Product,
                    _options.StsOptions.EndPoint);
                var client = new DefaultAcsClient(profile);
                var request = new AssumeRoleRequest
                {
                    AcceptFormat = Aliyun.Acs.Core.Http.FormatType.JSON,
                    RoleArn = _options.StsOptions.RoleArn,
                    RoleSessionName = _options.StsOptions.RoleSessionName,
                    DurationSeconds = _options.StsOptions.Expiration
                };
                return client.GetAcsResponse(request)?.Credentials;
            });
        }

        public async Task<dynamic> GetPolicyAsync(int objectType)
        {
            return await Task.Run(() =>
            {
                var expireTime = DateTime.UtcNow.AddSeconds(_options.PolicyOptions.ExpireTime);
                var config = new
                {
                    //OSS 大小写敏感，切勿修改
                    expiration = expireTime.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.CurrentCulture),
                    conditions = new List<IList<object>> { new List<object>() }
                };
                config.conditions[0].Add("content-length-range");
                config.conditions[0].Add(0);
                config.conditions[0].Add(1048576000);
                config.conditions.Add(new List<object>());
                config.conditions[1].Add("starts-with");
                config.conditions[1].Add("$key");
                var dir = _options[objectType].UploadDir;

                config.conditions[1].Add(dir);

                var policy = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(config)));

                using var algorithm = new HMACSHA1
                    { Key = Encoding.UTF8.GetBytes(_options.PolicyOptions.AccessKeySecret) };
                var signature = Convert.ToBase64String(
                    algorithm.ComputeHash(Encoding.UTF8.GetBytes(policy)));

                var callback = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                {
                    CallbackUrl = _options.PolicyOptions.CallbackUrl,
                    CallbackBody =
                        "bucket=${bucket}&filename=${object}&size=${size}&mimeType=${mimeType}&height=${imageInfo.height}&width=${imageInfo.width}",
                    CallbackBodyType = "application/x-www-form-urlencoded"
                })));
                return new
                {
                    AccessId = _options.PolicyOptions.AccessKeyId,
                    Host = _options.PolicyOptions.Host,
                    Policy = policy,
                    Signature = signature,
                    Expire = ((expireTime.Ticks - 621355968000000000) / 10000000L)
                        .ToString(CultureInfo.InvariantCulture),
                    Callback = callback,
                    Dir = dir
                };
            });
        }

        public async Task<OssObject> CallbackAsync(HttpRequest request)
        {
            if (!await request.VerifyOssSignatureAsync(_options.StsOptions.PublicKeyIssuers, HttpClient))
                // throw new OssException("invalid signature");
                return null;

            var obj = request.ExtractOssObject();
            obj.Verify(_oss, _options[obj.MimeType]);
            return await Task.FromResult(obj);
        }

        public Task<IEnumerable<OssObjectSummary>> ListObjectsAsync()
        {
            //https://help.aliyun.com/document_detail/187544.htm?spm=a2c4g.11186623.0.0.33bc4f695Jg7o0#reference-2520881
            return Task.FromResult(_oss.ListObjects(_options.PolicyOptions.BucketName).ObjectSummaries);
        }

        public async Task DownloadAsync(string objectName, string filename)
        {
            var obj = _oss.GetObject(_options.PolicyOptions.BucketName, objectName);
            await using var request = obj.Content;
            var buf = new byte[5 * 1024];
            await using var stream = File.OpenWrite(filename);
            var len = 0;
            while ((len = await request.ReadAsync(buf, 0, buf.Length)) != 0)
                stream.Write(buf, 0, len);
        }

        public async Task<Stream> DownloadAsync(string objectName)
        {
            var obj = _oss.GetObject(_options.PolicyOptions.BucketName, objectName);
            await using var request = obj.Content;
            var buf = new byte[5 * 1024];
            var stream = new MemoryStream();
            var len = 0;
            while ((len = await request.ReadAsync(buf, 0, buf.Length)) != 0)
                stream.Write(buf, 0, len);
            return stream;
        }

        public Task<DeleteObjectResult> DeleteObjectAsync(string objectName) =>
            Task.FromResult(_oss.DeleteObject(_options.PolicyOptions.BucketName, objectName));

        public Task<DeleteObjectsResult> DeleteObjectsAsync(IList<string> objectNames) =>
            Task.FromResult(
                _oss.DeleteObjects(new DeleteObjectsRequest(_options.PolicyOptions.BucketName, objectNames)));
    }
}