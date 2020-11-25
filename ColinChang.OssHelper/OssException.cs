using System;

namespace ColinChang.OssHelper
{
    public class OssException : Exception
    {
        public OssException(string message) : base(message)
        {
        }
    }
}