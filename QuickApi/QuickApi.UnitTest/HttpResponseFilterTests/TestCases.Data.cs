using QuickApi.HttpResponse;

namespace QuickApi.UnitTest.HttpResponseFilterTests;

public partial class TestCases
{
    internal class TestController
    {
        public string Get()
        {
            return "Hello World";
        }
    }
    [IgnoreResponseWrapper]
    internal class TestControllerWithIgnoreAttribute
    {
        public string Get()
        {
            return "Hello World";
        }
    }
}