using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace HtmlTags
{
    public class HtmlTag
    {
        private readonly List<HtmlTag> _children = new List<HtmlTag>();
        private readonly List<string> _cssClasses = new List<string>();
        private readonly Cache<string, string> _customStyles = new Cache<string, string>();

        private readonly Cache<string, string> _htmlAttributes =
            new Cache<string, string>(
                key => { throw new KeyNotFoundException("Does not have html attribute '{0}'".ToFormat(key)); });

        private readonly Cache<string, object> _metaData = new Cache<string, object>();
        private string _innerText = string.Empty;
        private bool _isVisible = true;
        private string _tag;

        public HtmlTag(string tag)
        {
            _tag = tag.ToLower();
        }

        public HtmlTag(string tag, Action<HtmlTag> configure)
            : this(tag)
        {
            configure(this);
        }

        public IList<HtmlTag> Children { get { return _children; } }

        public string TagName()
        {
            return _tag;
        }

        public HtmlTag TagName(string tag)
        {
            _tag = tag.ToLower();
            return this;
        }

        public HtmlTag Prepend(string text)
        {
            _innerText = text + _innerText;
            return this;
        }

        public HtmlTag FirstChild()
        {
            return _children.FirstOrDefault();
        }

        public void InsertFirst(HtmlTag tag)
        {
            _children.Insert(0, tag);
        }

        public HtmlTag Add(string tag, params string[] classes)
        {
            string[] tags = tag.ToDelimitedArray('/');
            HtmlTag element = this;
            tags.Each(x =>
            {
                var child = new HtmlTag(x);
                element.Child(child);

                element = child;
            });

            element.AddClasses(classes);
            return element;
        }

        public HtmlTag Add(string tag, Action<HtmlTag> configuration)
        {
            HtmlTag element = Add(tag);
            configuration(element);

            return element;
        }

        public HtmlTag Style(string key, string value)
        {
            _customStyles[key] = value;
            return this;
        }

        public HtmlTag Id(string id)
        {
            return Attr("id", id);
        }

        public string Id()
        {
            return _htmlAttributes["id"];
        }

        public HtmlTag Hide()
        {
            return Style("display", "none");
        }

        public HtmlTag Child(HtmlTag child)
        {
            _children.Add(child);
            return this;
        }

        public T Child<T>() where T : HtmlTag, new()
        {
            var child = new T();
            _children.Add(child);

            return child;
        }

        public HtmlTag MetaData(string key, object value)
        {
            _metaData[key] = value;
            return this;
        }

        public object MetaData(string key)
        {
            return _metaData[key];
        }

        public HtmlTag Text(string text)
        {
            _innerText = text;
            return this;
        }

        public string Text()
        {
            return _innerText;
        }


        public HtmlTag With(Action<HtmlTag> action)
        {
            action(this);
            return this;
        }


        public override string ToString()
        {
            return ToString("  ");
        }

        public string ToString(string tab)
        {
            var html = new HtmlTextWriter(new StringWriter(), tab);
            html.Indent = 2;

            writeHtml(html);

            return html.InnerWriter.ToString();
        }

        public string ToCompacted()
        {
            return ToString(null).Replace("\r\n", string.Empty);
        }


        private void writeHtml(HtmlTextWriter html)
        {
            if (!_isVisible) return;

            _htmlAttributes.Each(html.AddAttribute);

            if (_cssClasses.Count > 0 || _metaData.Count > 0)
            {
                string classAttribute = toClassArray().Join(" ");
                html.AddAttribute("class", classAttribute);
            }

            if (_customStyles.Count > 0)
            {
                string attValue = _customStyles.Inner
                    .Select(x => x.Key + ":" + x.Value)
                    .ToArray().Join(";");

                html.AddAttribute("style", attValue);
            }

            html.RenderBeginTag(_tag);

            if (_innerText != null)
            {
                html.WriteEncodedText(_innerText);
            }

            _children.Each(x => x.writeHtml(html));

            html.RenderEndTag();
        }

        private string[] toClassArray()
        {
            if (_metaData.Count == 0) return _cssClasses.ToArray();

            string metaDataClass = JsonUtil.ToUnsafeJson(_metaData.Inner).Replace('"', '\'');
            return _cssClasses.ToArray().Union(new[] {metaDataClass}).ToArray();
        }

        public HtmlTag Attr(string attribute, object value)
        {
            _htmlAttributes[attribute] = value == null ? string.Empty : value.ToString();
            return this;
        }

        public string Attr(string attribute)
        {
            return _htmlAttributes[attribute];
        }

        public HtmlTag Visible(bool isVisible)
        {
            _isVisible = isVisible;
            return this;
        }

        public bool Visible()
        {
            return _isVisible;
        }

        public HtmlTag AddClass(string className)
        {
            className = className.Replace(" ", string.Empty);

            if (!_cssClasses.Contains(className))
            {
                _cssClasses.Add(className);
            }

            return this;
        }

        public HtmlTag AddClasses(params string[] classes)
        {
            classes.Each(x => AddClass(x));
            return this;
        }

        public HtmlTag AddClasses(IList<string> classes)
        {
            AddClasses(classes.ToArray());
            return this;
        }

        public bool HasClass(string className)
        {
            return _cssClasses.Contains(className);
        }

        public bool HasMetaData(string key)
        {
            return _metaData.Has(key);
        }

        public string Title()
        {
            return Attr("title");
        }

        public HtmlTag Title(string title)
        {
            return Attr("title", title);
        }

        public bool HasAttr(string key)
        {
            return _htmlAttributes.Has(key);
        }

        public bool IsInputElement()
        {
            return _tag == "input" || _tag == "select";
        }
    }

    public static class TagExtensions
    {
        public static HtmlTag Span(this HtmlTag tag, Action<HtmlTag> configure)
        {
            var span = new HtmlTag("span");
            configure(span);
            return tag.Child(span);
        }

        public static HtmlTag Div(this HtmlTag tag, Action<HtmlTag> configure)
        {
            var div = new HtmlTag("div");
            configure(div);

            return tag.Child(div);
        }

        public static LinkTag ActionLink(this HtmlTag tag, string text, params string[] classes)
        {
            var child = new LinkTag(text, "#", classes);
            tag.Child(child);

            return child;
        }
    }
}