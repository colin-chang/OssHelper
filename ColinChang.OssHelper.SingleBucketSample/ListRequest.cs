namespace ColinChang.OssHelper.SingleBucketSample;

public class ListRequest
{
    public string Prefix { get; set; }
    public string Mark { get; set; }
    public int? MaxKeys { get; set; }
    public string Delimiter { get; set; }
}