using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aliyun.Acs.Core.Auth.Sts;
using Autofac.Features.Indexed;

namespace ColinChang.OssHelper.MultiBucketSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OssController : ControllerBase
    {
        private readonly IIndex<string, IOssHelper> _buckets;
        public OssController(IIndex<string, IOssHelper> buckets) => _buckets = buckets;

        /// <summary>
        /// STS
        /// </summary>
        /// <returns></returns>
        [HttpGet("{bucket}")]
        public async Task<AssumeRoleResponse.AssumeRole_Credentials> PostAsync([FromRoute] string bucket) =>
            await _buckets[bucket].GetStsAsync();
    }
}