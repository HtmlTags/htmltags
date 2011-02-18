using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class JsonUtilTester
    {
        [Test]
        public void should_serialize_strings_with_quotes()
        {
            JsonUtil.ToJson("onSuccess").ShouldEqual("\"onSuccess\"");
        }

        [Test]
        public void unsafe_json_should_allow_you_to_serialize_javascript_function_names_without_quotes()
        {
            JsonUtil.ToUnsafeJson(javascript.function("onSuccess")).ShouldEqual("onSuccess");
        }
    }
}