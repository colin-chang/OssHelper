using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aliyun.Acs.Core.Auth.Sts;
using Aliyun.OSS;
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
        Task<dynamic> GetPolicyAsync(int category);

        /// <summary>
        /// OSS 通用回调
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<OssObject> CallbackAsync(HttpRequest request);

        /// <summary>
        /// 列举文件
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OssObjectSummary>> ListObjectsAsync();

        /// <summary>
        /// 流式下载(如果要下载的文件太大，或者一次性下载耗时太长，您可以通过流式下载，一次处理部分内容，直到完成文件的下载)
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="filename">本地文件存储</param>
        /// <returns></returns>
        Task DownloadAsync(string objectName, string filename);

        /// <summary>
        /// 流式下载(如果要下载的文件太大，或者一次性下载耗时太长，您可以通过流式下载，一次处理部分内容，直到完成文件的下载)
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        Task<Stream> DownloadAsync(string objectName);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        Task<DeleteObjectResult> DeleteObjectAsync(string objectName);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="objectNames"></param>
        /// <returns></returns>
        Task<DeleteObjectsResult> DeleteObjectsAsync(IList<string> objectNames);
    }
}