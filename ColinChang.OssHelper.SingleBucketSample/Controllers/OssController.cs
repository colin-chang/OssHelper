using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aliyun.Acs.Core.Auth.Sts;

namespace ColinChang.OssHelper.SingleBucketSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OssController : ControllerBase
    {
        private readonly IOssHelper _oss;

        public OssController(IOssHelper oss) => _oss = oss;

        /// <summary>
        /// STS
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<AssumeRoleResponse.AssumeRole_Credentials> PostAsync() => await _oss.GetStsAsync();

        /// <summary>
        /// Policy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("{type}")]
        public async Task<dynamic> GetAsync([FromRoute] ObjectType type)
        {
            Response.ContentType = "application/json;";
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            Response.Headers.Add("Access-Control-Allow-Method", "GET, POST");
            Response.Headers.Add("Connection", "close");
            return Ok(await _oss.GetPolicyAsync(type));
        }

        /// <summary>
        /// Callback
        /// </summary>
        /// <returns></returns>
        [HttpPost("CallBack")]
        public async Task<IActionResult> CallbackAsync()
        {
            var obj = await _oss.CallbackAsync(Request);
            // obj.Object

            return StatusCode((int) (obj != null ? HttpStatusCode.OK : HttpStatusCode.Forbidden));
        }

        [HttpGet]
        public async Task DownloadAsync([FromQuery] string objectName)
        {
            var filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(objectName));
            await _oss.DownloadAsync(objectName,filename);
        }
    }
}