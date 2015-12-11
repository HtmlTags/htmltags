using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Should;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class ParentTagTester
    {
        [Fact]
        public void parent_property_is_set_correctly_using_add()
        {
            var tag = new HtmlTag("div");
            var child = tag.Add("span");
            tag.ShouldEqual(child.Parent);
            tag.Children[0].ShouldEqual(child);
        }

        [Fact]
        public void parent_property_is_set_correctly_using_append()
        {
            var child = new HtmlTag("span");
            var tag = new HtmlTag("div").Append(child);
            tag.ShouldEqual(child.Parent);
            tag.Children[0].ShouldEqual(child);
        }

        [Fact]
        public void parent_property_is_set_correctly_using_append_with_multiple()
        {
            var child = new HtmlTag("span");
            var child1 = new HtmlTag("span");
            var tag = new HtmlTag("div").Append(new List<HtmlTag>() { child, child1 });
            tag.ShouldEqual(child.Parent);
            tag.ShouldEqual(child1.Parent);
            tag.Children[0].ShouldEqual(child);
            tag.Children[1].ShouldEqual(child1);
        }

        [Fact]
        public void parent_property_is_set_correctly_using_insertFirst()
        {
            var child = new HtmlTag("span");
            var tag = new HtmlTag("div");
            tag.InsertFirst(child);
            tag.ShouldEqual(child.Parent);
            tag.Children[0].ShouldEqual(child);
        }

        [Fact]
        public void render_html_from_current_tag_by_default()
        {
            var tag = new HtmlTag("div");
            var child = tag.Add("span").Text("hi");
            child.ToString().ShouldEqual("<span>hi</span>");
        }
    
        [Fact]
        public void render_html_from_top_if_set_renderfromtop()
        {
            var tag = new HtmlTag("div");
            var child = tag.Add("span").RenderFromTop().Text("hi");
            child.ToString().ShouldEqual("<div><span>hi</span></div>");
        }
    }
}
