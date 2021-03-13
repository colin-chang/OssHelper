using System.Collections.Generic;

namespace ColinChang.OssHelper.MultiBucket
{
    public class OssBuckets
    {
        public Dictionary<string, OssHelperOptions> Buckets { get; set; }

        public OssBuckets(Dictionary<string, OssHelperOptions> buckets) =>
            Buckets = buckets;
    }
}