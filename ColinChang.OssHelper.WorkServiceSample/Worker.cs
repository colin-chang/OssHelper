using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ColinChang.OssHelper.WorkServiceSample
{
    public class Worker : BackgroundService
    {
        private readonly IIndex<string, IOssHelper> _buckets;
        private readonly ILogger _logger;

        public Worker(IIndex<string, IOssHelper> buckets, ILogger<Worker> logger)
        {
            _buckets = buckets;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var sts0 = await _buckets["Bucket0"].GetStsAsync();
            var sts1 = await _buckets["Bucket1"].GetStsAsync();
            
            _logger.LogInformation($"{nameof(sts0)}:{JsonSerializer.Serialize(sts0)}");
            _logger.LogInformation($"{nameof(sts1)}:{JsonSerializer.Serialize(sts1)}");
        }
    }
}