using System.Threading.Tasks;
using Aliyun.Acs.Core.Auth.Sts;
using Microsoft.AspNetCore.Http;

namespace ColinChang.OssHelper
{
    public interface IOssHelper
    {
        /// <summary>
        /// 阿里云OSS STS授权
        /// </summary>
        /// <returns></returns>
        Task<AssumeRoleResponse.AssumeRole_Credentials> GetStsAsync();

        /// <summary>
        /// 阿里云OSS Policy授权
        /// </summary>
        /// <returns></returns>
        Task<dynamic> GetPolicyAsync(ObjectType category);

        /// <summary>
        /// OSS 通用回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OssObject> CallbackAsync(HttpRequest request);
    }
}