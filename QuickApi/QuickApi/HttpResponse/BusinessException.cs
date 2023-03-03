using System;
using System.Net;

namespace QuickApi.HttpResponse
{
    public class BusinessException:Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public int BusinessCode { get; set; }
        
        public BusinessException(string message,int businessCode=500, HttpStatusCode statusCode = HttpStatusCode.OK) : base(message)
        {
            BusinessCode=businessCode;
            StatusCode = statusCode;
        }
    }
}