using System.Text;
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

        [Test]
        public void get_by_bytes()
        {
            var json = JsonUtil.ToJson(new JsonUtilTarget{
                Name = "Jeremy"
            });
            var bytes = Encoding.Default.GetBytes(json);

            JsonUtil.Get<JsonUtilTarget>(bytes).Name.ShouldEqual("Jeremy");
        }
    }

    public class JsonUtilTarget
    {
        public string Name { get; set; }
    }
}