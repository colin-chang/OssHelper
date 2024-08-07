using Aliyun.Acs.Core.Auth.Sts;

namespace ColinChang.OssHelper
{
    public class OssObject
    {
        public string Bucket { get; set; }
        public string Object { get; set; }
        public long Size { get; set; }
        public string MimeType { get; set; }
    }

    public class StsCredentials : AssumeRoleResponse.AssumeRole_Credentials
    {
        public string BucketName { get; set; }
    }
}