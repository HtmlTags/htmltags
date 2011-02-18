using System;
using NUnit.Framework;

namespace HtmlTags.Testing
{
    [TestFixture]
    public class HtmlDocumentTester
    {
        [SetUp]
        public void SetUp()
        {
            document = new HtmlDocument();
            document.Title = "the title";
        }

        private HtmlDocument document;

        [Test]
        public void add_does_not_push_onto_stack()
        {
            document.Add("div/a").Text("hello");
            document.Current.TagName().ShouldEqual("body");
        }

        [Test]
        public void add_styling()
        {
            document.AddStyle("some styling");
            document.ToCompacted().ShouldContain("<style>some styling</style>");
        }

        [Test]
        public void check_the_basic_structure_with_title_body_and_head()
        {
            document.ToCompacted().ShouldEqual("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" + Environment.NewLine +
                "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>the title</title></head><body></body></html>");
        }

        [Test]
        public void pop_rewinds_the_current()
        {
            document.Push("div/span");
            document.Pop();

            document.Current.TagName().ShouldEqual("body");
        }

        [Test]
        public void push_adds_to_the_stack()
        {
            HtmlTag element = document.Push("div/span").Text("hello");
            document.Current.ShouldBeTheSameAs(element);
            document.ToCompacted().ShouldContain("<body><div><span>hello</span></div></body>");
        }

        [Test]
        public void the_current_element_is_the_body_by_default()
        {
            document.Current.TagName().ShouldEqual("body");
        }

        [Test]
        public void the_last_property()
        {
            document.Last.TagName().ShouldEqual("body");
            document.Add("table/tr/td");
            document.Last.TagName().ShouldEqual("td");

            document.Push("p");

            document.Last.TagName().ShouldEqual("p");
        }
    }
}