using System;
using System.Linq.Expressions;
using HtmlTags.Reflection;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements.Builders;
using Should;
using Xunit;

namespace HtmlTags.Testing
{
    
    public class CheckboxTagTester
    {
        [Fact]
        public void basic_construction()
        {
            var tag = new CheckboxTag(true);
            tag.TagName().ShouldEqual("input");
            tag.Attr("type").ShouldEqual("checkbox");
        }

        [Fact]
        public void create_checkbox_that_is_checked()
        {
            var tag = new CheckboxTag(true);
            tag.Attr("checked").ShouldEqual("true");
        }

        [Fact]
        public void create_checkbox_that_is_not_checked()
        {
            var tag = new CheckboxTag(false);
            tag.HasAttr("checked").ShouldBeFalse();
        }

        [Fact]
        public void Should_build_with_null()
        {
            var builder = new CheckboxBuilder();
            Expression<Func<Model, object>> m = _ => _.Toggle;
            var accessor = m.ToAccessor();
            var tag = builder.Build(new ElementRequest(accessor));
            tag.TagName().ShouldEqual("input");
            tag.Attr("type").ShouldEqual("checkbox");
            tag.HasAttr("checked").ShouldBeFalse();
        }

        private class Model
        {
            public bool Toggle { get; set; }
        }
    }
}