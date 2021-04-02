using Autofac;
using Autofac.Extensions.DependencyInjection;
using ColinChang.OssHelper.MultiBucket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ColinChang.OssHelper.WorkServiceSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureServices(services =>
                {
                    services.AddHttpClient();
                    services.AddHostedService<Worker>();
                })
                .ConfigureContainer<ContainerBuilder>(
                    (context, builder) =>
                        builder.RegisterOssHelpers(context.Configuration.GetSection(nameof(OssBuckets))));
    }
}