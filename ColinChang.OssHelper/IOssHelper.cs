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
        /// <param name="prefix">限定返回文件的Key必须以prefix作为前缀。如果把prefix设为某个文件夹名，则列举以此prefix开头的文件，即该文件夹下递归的所有文件和子文件夹。如果把prefix设为某个文件夹名，则列举以此prefix开头的文件，即该文件夹下递归的所有文件和子文件夹。</param>
        /// <param name="marker">指定List操作需要从此文件开始</param>
        /// <param name="maxKeys">指定返回Object的最大数。</param>
        /// <param name="delimiter">对Object名字进行分组的字符。所有Object名字包含指定的前缀，第一次出现delimiter字符之间的Object作为一组元素</param>
        /// <returns></returns>
        Task<ObjectListing> ListObjectsAsync(string prefix = null, string marker = null,
            int? maxKeys = null, string delimiter = null);

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

        /// <summary>
        /// 上传对象
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="objectType">对象类型. 0:Photo, 1:Video, 2:Application, 3:Other</param>
        /// <param name="data">上传内容</param>
        /// <returns></returns>
        Task<PutObjectResult> PutObjectAsync(string fileName, int objectType, byte[] data);
    }
}