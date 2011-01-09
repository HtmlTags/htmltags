using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace HtmlTags
{
    public interface ITagSource
    {
        IEnumerable<HtmlTag> AllTags();
    }

    public class TagList : ITagSource
#if !LEGACY
        , IHtmlString
#endif
    {
        private readonly IEnumerable<HtmlTag> _tags;

        public TagList(IEnumerable<HtmlTag> tags)
        {
            _tags = tags;
        }

        public string ToHtmlString()
        {
            if (_tags.Count() > 5)
            {
                var builder = new StringBuilder();
                _tags.Each(t => builder.AppendLine(t.ToString()));

                return builder.ToString();
            }

            return _tags.Select(x => x.ToString()).ToArray().Join("\n");
        }

        public IEnumerable<HtmlTag> AllTags()
        {
            return _tags;
        }
    }

    public class HtmlTag : ITagSource
#if !LEGACY
        , IHtmlString
#endif
    {
        public static HtmlTag Empty()
        {
            return new HtmlTag("span").Visible(false);
        }

        private readonly List<HtmlTag> _children = new List<HtmlTag>();
        private readonly HashSet<string> _cssClasses = new HashSet<string>();
        private readonly IDictionary<string, string> _customStyles = new Dictionary<string, string>();

        private readonly Cache<string, string> _htmlAttributes =
            new Cache<string, string>(
                key => { throw new KeyNotFoundException("Does not have html attribute '{0}'".ToFormat(key)); });

        private readonly Cache<string, object> _metaData = new Cache<string, object>();
        private string _innerText = string.Empty;
        private bool _isVisible = true;
        private string _tag;
        private bool _ignoreClosingTag;
        private bool _isAuthorized = true;

        private HtmlTag()
        {
            EncodeInnerText = true;
        }

        public HtmlTag(string tag) : this()
        {
            _tag = tag.ToLower();
        }

        public HtmlTag(string tag, Action<HtmlTag> configure)
            : this(tag)
        {
            configure(this);
        }

        protected bool EncodeInnerText { get; set; }

        public HtmlTag Next { get; set; }
        
        public IList<HtmlTag> Children { get { return _children; } }

        IEnumerable<HtmlTag> ITagSource.AllTags()
        {
            yield return this;
        }

        public HtmlTag AddChildren(ITagSource tags)
        {
            _children.AddRange(tags.AllTags());
            return this;
        }

        public HtmlTag AddChildren(IEnumerable<HtmlTag> tags)
        {
            _children.AddRange(tags);
            return this;
        }

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

        public HtmlTag Add(string tag)
        {
            string[] tags = tag.ToDelimitedArray('/');
            HtmlTag element = this;
            tags.Each(x =>
            {
                var child = new HtmlTag(x);
                element.Child(child);

                element = child;
            });

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

        public string Style(string key)
        {
            return _customStyles[key];
        }

        public bool HasStyle(string key)
        {
            return _customStyles.ContainsKey(key);
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

        public HtmlTag MetaData<T>(string key, Action<T> configure) where T : class
        {
            if (!_metaData.Has(key)) return this;
            var value = (T)_metaData[key];
            configure(value);

            return this;
        }

        public object MetaData(string key)
        {
            return !_metaData.Has(key) ? null : _metaData[key];
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


        public HtmlTag Modify(Action<HtmlTag> action)
        {
            action(this);
            return this;
        }

        public HtmlTag Authorized(bool isAuthorized)
        {
            _isAuthorized = isAuthorized;
            return this;
        }

        public bool Authorized()
        {
            return _isAuthorized;
        }


        public override string ToString()
        {
            return ToString(new HtmlTextWriter(new StringWriter(), string.Empty){NewLine = string.Empty});
        }

        public string ToHtmlString()
        {
            return ToString();
        }

        public string ToPrettyString()
        {
            return ToString(new HtmlTextWriter(new StringWriter(), "  ") { NewLine = Environment.NewLine });
        }


        public string ToString(HtmlTextWriter html)
        {
            writeHtml(html);
            return html.InnerWriter.ToString();
        }

        public bool WillBeRendered()
        {
            return _isVisible && _isAuthorized;
        }

        private void writeHtml(HtmlTextWriter html)
        {
            if (!_isVisible) return;
            if (!_isAuthorized) return;

            _htmlAttributes.Each(html.AddAttribute);

            if (_cssClasses.Count > 0 || _metaData.Count > 0)
            {
                var classAttribute = toClassArray().Join(" ");
                html.AddAttribute("class", classAttribute);
            }

            if (_customStyles.Count > 0)
            {
                var attValue = _customStyles
                    .Select(x => x.Key + ":" + x.Value)
                    .ToArray().Join(";");

                html.AddAttribute("style", attValue);
            }

            html.RenderBeginTag(_tag);

            writeInnerText(html);

            _children.Each(x => x.writeHtml(html));

            if (!_ignoreClosingTag)
            {
                html.RenderEndTag();
            }

            if (Next != null) Next.writeHtml(html);
        }

        private void writeInnerText(HtmlTextWriter html)
        {
            if (_innerText == null) return;

            if (EncodeInnerText)
            {
                html.WriteEncodedText(_innerText);
            }
            else
            {
                html.Write(_innerText);
            }
        }

        private string[] toClassArray()
        {
            if (_metaData.Count == 0) return _cssClasses.ToArray();

            var metaDataClass = JsonUtil.ToUnsafeJson(_metaData.Inner);
            return _cssClasses.Concat(new[] {metaDataClass}).ToArray();
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

        public HtmlTag RemoveAttr(string attribute)
        {
            _htmlAttributes.Remove(attribute);
            return this;
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
            if (isInvalidClassName(className)) throw new ArgumentException("CSS class names cannot contain spaces. If you are attempting to add multiple classes, call AddClasses() instead. Problem class was '{0}'".ToFormat(className), "className");

            _cssClasses.Add(className);

            return this;
        }

        private static bool isInvalidClassName(string className)
        {
            if (className.StartsWith("{") && className.EndsWith("}")) return false;

            return className.Contains(" ");
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

        public IEnumerable<string> GetClasses()
        {
            return _cssClasses;
        }

        public bool HasClass(string className)
        {
            return _cssClasses.Contains(className);
        }

        public HtmlTag RemoveClass(string className)
        {
            _cssClasses.Remove(className);
            return this;
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
            return _tag == "input" || _tag == "select" || _tag == "textarea";
        }

        public void ReplaceChildren(params HtmlTag[] tags)
        {
            Children.Clear();
            tags.Each(t => Children.Add(t));
        }

        public HtmlTag NoClosingTag()
        {
            _ignoreClosingTag = true;
            return this;
        }


        public HtmlTag WrapWith(string tag)
        {
            var wrapper = new HtmlTag(tag);
            wrapper.Child(this);

            // Copies visibility and authorization from inner tag
            wrapper.Visible(Visible());
            wrapper.Authorized(Authorized());

            return wrapper;
        }

        public HtmlTag WrapWith(HtmlTag wrapper)
        {
            wrapper.InsertFirst(this);
            return wrapper;
        }

        public HtmlTag VisibleForRoles(params string[] roles)
        {
            var principal = findPrincipal();
            return Visible(roles.Any(r => principal.IsInRole(r)));
        }

        private IPrincipal findPrincipal()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.User;
            }

            // Rather throw up on nulls than put a fake in
            return Thread.CurrentPrincipal;
        }

        public HtmlTag UnEncoded()
        {
            EncodeInnerText = false;
            return this;
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