using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ColinChang.OssHelper
{
    public class OssHelperOptions
    {
        public StsOptions StsOptions { get; set; }
        public PolicyOptions PolicyOptions { get; set; }
        public ObjectOption[] ObjectOptions { get; set; }

        public ObjectOption this[int objectType] =>
            ObjectOptions.FirstOrDefault(o => o.ObjectType == objectType);

        public ObjectOption this[string mimeType] =>
            ObjectOptions.FirstOrDefault(o => o.AllowedMimeTypes.Contains(mimeType));
    }

    public abstract class AliyunCommonOptions
    {
        public string BucketName { get; set; }
        [Required] public string AccessKeyId { get; set; }
        [Required] public string AccessKeySecret { get; set; }
        [Required] public string EndPoint { get; set; }
    }


    public class StsOptions : AliyunCommonOptions
    {
        [Required] public string RegionId { get; set; }
        [Required] public string Product { get; set; }
        [Required] public string RoleArn { get; set; }

        public string RoleSessionName { get; set; }

        public long Expiration { get; set; }
        public string[] PublicKeyIssuers { get; set; }
    }

    public class PolicyOptions : AliyunCommonOptions
    {
        [Required] public string BucketUrl { get; set; }
        [Required] public string Host { get; set; }
        [Required] public string CallbackUrl { get; set; }
        [Required] public int ExpireTime { get; set; }
    }

    public class ObjectOption
    {
        public int ObjectType { get; set; }
        public string UploadDir { get; set; }
        public long MaxSize { get; set; }
        public IEnumerable<string> AllowedMimeTypes { get; set; }
    }
}