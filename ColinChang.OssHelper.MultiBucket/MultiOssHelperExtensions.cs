using System.Collections.Generic;
using Autofac;
using AutoMapper;
using ColinChang.OssHelper.MultiBucket;
using Microsoft.Extensions.Configuration;

namespace ColinChang.OssHelper
{
    public static class MultiOssHelperExtensions
    {
        public static void RegisterOssHelpers(this ContainerBuilder builder, IConfiguration buckets)
        {
            var dict = buckets.GetMultiOssHelperOptions().Buckets;
            foreach (var (key, options) in dict)
                builder.Register(ctx => new OssHelper(options)).Keyed<IOssHelper>(key).PropertiesAutowired();
        }

        private static OssBuckets GetMultiOssHelperOptions(
            this IConfiguration ossBuckets)
        {
            var buckets = ossBuckets.GetChildren();
            var dict = new Dictionary<string, OssHelperOptions>();
            var mapper = new MapperConfiguration(cfg =>
                    cfg.CreateMap<MultiOssHelperOptions, OssHelperOptions>())
                .CreateMapper();
            foreach (var bucket in buckets)
            {
                var bucketOpt = bucket.Get<MultiOssHelperOptions>();
                dict[bucketOpt.Key] = mapper.Map<OssHelperOptions>(bucketOpt);
            }

            return new OssBuckets(dict);
        }
    }
}