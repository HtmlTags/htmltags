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
        public void head_contains_the_title()
        {
            document.Head.FirstChild().Text().ShouldBeTheSameAs(document.Title);
            document.ToString().ShouldContain("<head><title>the title</title></head>");
        }

        [Test]
        public void add_does_not_push_onto_stack()
        {
            document.Add("div/a").Text("hello");
            document.Current.TagName().ShouldEqual("body");
        }

        [Test]
        public void add_a_tag_by_tag_name()
        {
            HtmlTag element = document.Add("div/a").Text("hello");
            element.TagName().ShouldEqual("a");
        }

        [Test]
        public void add_an_htmlTag()
        {
            document.Add(new HtmlTag("p"));
            document.Last.TagName().ShouldEqual("p");
            document.Current.TagName().ShouldEqual("body");
            document.Current.FirstChild().TagName().ShouldEqual("p");
        }

        [Test]
        public void add_tags_from_a_tag_source()
        {
            var tagList = new TagList(
                new[]
                    {
                        new HtmlTag("h1"),
                        new HtmlTag("div"),
                        new HtmlTag("p")
                    });
            document.Add(tagList);
            document.Body.Children.ShouldHaveCount(3);
            document.Current.TagName().ShouldEqual("body");
            document.Last.TagName().ShouldEqual("p");
        }

        [Test]
        public void add_styling()
        {
            document.AddStyle("some styling");
            document.ToString().ShouldContain("</title><style>some styling</style></head>");
        }

        [Test]
        public void reference_external_stylesheet()
        {
            var path = "css/site.css";
            document.ReferenceStyle(path);
            document.ToString().ShouldContain("</title><link media=\"screen\" href=\"css/site.css\" type=\"text/css\" rel=\"stylesheet\" /></head>");
        }

        [Test]
        public void check_the_basic_structure_with_title_body_and_head()
        {
            document.ToString().ShouldEqual("<!DOCTYPE html>" + Environment.NewLine +
                "<html><head><title>the title</title></head><body></body></html>");
        }

        [Test]
        public void can_set_a_legacy_doctype()
        {
            document.DocType =
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            document.RootTag.Attr("xmlns", "http://www.w3.org/1999/xhtml");

            document.ToString().ShouldEqual("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">" + Environment.NewLine +
               "<html xmlns=\"http://www.w3.org/1999/xhtml\"><head><title>the title</title></head><body></body></html>");
        }

        [Test]
        public void add_javascript()
        {
            document.AddJavaScript(@"
alert('hello');
alert('world');
");
            var expected = String.Format(
                "</title><script type=\"text/javascript\">{0}{0}alert('hello');{0}alert('world');{0}{0}</script></head>",
                Environment.NewLine);
            document.ToString().ShouldContain(expected);
        }

        [Test]
        public void reference_javascript_by_file()
        {
            var path = "scripts/myfile.js";
            document.ReferenceJavaScriptFile(path);
            document.ToString().ShouldContain("</title><script type=\"text/javascript\" language=\"javascript\" src=\"" + path + "\"></script></head>");
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
            document.ToString().ShouldContain("<body><div><span>hello</span></div></body>");
        }

        [Test]
        public void push_an_htmlTag()
        {
            var element = new HtmlTag("p").Text("a paragraph");
            document.Push(element);
            document.Current.ShouldBeTheSameAs(element);
            document.ToString().ShouldContain("<body><p>a paragraph</p></body>");
        }

        [Test]
        public void push_without_attaching_does_not_add_to_children()
        {
            HtmlTag attachedTag = document.Push("div");
            HtmlTag unattachedTag = new HtmlTag("span");
            document.PushWithoutAttaching(unattachedTag);
            document.Body.FirstChild().ShouldBeTheSameAs(attachedTag);
            document.Current.ShouldBeTheSameAs(unattachedTag);
            document.ToString().ShouldEndWith("<body><div></div></body></html>");
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

        [Test]
        public void write_to_file_sends_the_document_to_the_file_writer()
        {
            string actualContent = null;
            document.FileWriter = (path, content) =>
                                      {
                                          actualContent = content;
                                      };

            document.WriteToFile("myfile.htm");

            actualContent.ShouldStartWith("<!DOCTYPE");
        }

        [Test]
        public void open_in_browser_writes_a_temp_file_then_opens_it()
        {
            string actualPath = null;
            string actualContent = null;
            string actualTempFileName = null;
            document.FileWriter = (path, content) =>
                                      {
                                          actualPath = path;
                                          actualContent = content;
                                      };
            document.FileOpener = (filename) =>
                                      {
                                          actualTempFileName = filename;
                                      };
            
            document.OpenInBrowser();

            actualPath.ShouldEqual(actualTempFileName);
            actualContent.ShouldStartWith("<!DOCTYPE");
        }
    }
}