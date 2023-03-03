namespace QuickApi.HttpResponse
{
    public class HttpResponseResult
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public bool success { get; set; }
    }
}