using System.Text;
using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
#if NETSTANDARD1_5
using System.Text.Encodings.Web;
#else
using System.Web;
#endif

namespace HtmlTags
{
    public class HtmlTextWriter : TextWriter
    {
        public enum HtmlTextWriterAttribute
        {
            Accesskey,
            Align,
            Alt,
            Background,
            Bgcolor,
            Border,
            Bordercolor,
            Cellpadding,
            Cellspacing,
            Checked,
            Class,
            Cols,
            Colspan,
            Disabled,
            For,
            Height,
            Href,
            Id,
            Maxlength,
            Multiple,
            Name,
            Nowrap,
            Onchange,
            Onclick,
            ReadOnly,
            Rows,
            Rowspan,
            Rules,
            Selected,
            Size,
            Src,
            Style,
            Tabindex,
            Target,
            Title,
            Type,
            Valign,
            Value,
            Width,
            Wrap,
            Abbr,
            AutoComplete,
            Axis,
            Content,
            Coords,
            DesignerRegion,
            Dir,
            Headers,
            Longdesc,
            Rel,
            Scope,
            Shape,
            Usemap,
            VCardName,
        }

        public enum HtmlTextWriterTag
        {
            Unknown,
            A,
            Acronym,
            Address,
            Area,
            B,
            Base,
            Basefont,
            Bdo,
            Bgsound,
            Big,
            Blockquote,
            Body,
            Br,
            Button,
            Caption,
            Center,
            Cite,
            Code,
            Col,
            Colgroup,
            Dd,
            Del,
            Dfn,
            Dir,
            Div,
            Dl,
            Dt,
            Em,
            Embed,
            Fieldset,
            Font,
            Form,
            Frame,
            Frameset,
            H1,
            H2,
            H3,
            H4,
            H5,
            H6,
            Head,
            Hr,
            Html,
            I,
            Iframe,
            Img,
            Input,
            Ins,
            Isindex,
            Kbd,
            Label,
            Legend,
            Li,
            Link,
            Map,
            Marquee,
            Menu,
            Meta,
            Nobr,
            Noframes,
            Noscript,
            Object,
            Ol,
            Option,
            P,
            Param,
            Pre,
            Q,
            Rt,
            Ruby,
            S,
            Samp,
            Script,
            Select,
            Small,
            Span,
            Strike,
            Strong,
            Style,
            Sub,
            Sup,
            Table,
            Tbody,
            Td,
            Textarea,
            Tfoot,
            Th,
            Thead,
            Title,
            Tr,
            Tt,
            U,
            Ul,
            Var,
            Wbr,
            Xml,
        }



        private TextWriter writer;
        private int indentLevel;
        private bool tabsPending;
        private string tabString;
        public const char TagLeftChar = '<';
        public const char TagRightChar = '>';
        public const string SelfClosingChars = " /";
        public const string SelfClosingTagEnd = " />";
        public const string EndTagLeftChars = "</";
        public const char DoubleQuoteChar = '"';
        public const char SingleQuoteChar = '\'';
        public const char SpaceChar = ' ';
        public const char EqualsChar = '=';
        public const char SlashChar = '/';
        public const string EqualsDoubleQuoteString = "=\"";
        public const char SemicolonChar = ';';
        public const char StyleEqualsChar = ':';
        public const string DefaultTabString = "\t";

        // The DesignerRegion attribute name must be kept in sync with
        // System.Web.UI.Design.DesignerRegion.DesignerRegionNameAttribute
        internal const string DesignerRegionAttributeName = "_designerRegion";

        private static Dictionary<string, HtmlTextWriterTag> _tagKeyLookupTable;
        private static Dictionary<string, HtmlTextWriterAttribute> _attrKeyLookupTable;
        private static TagInformation[] _tagNameLookupArray;
        private static AttributeInformation[] _attrNameLookupArray;

        private RenderAttribute[] _attrList;
        private int _attrCount;
        private int _endTagCount;
        private TagStackEntry[] _endTags;
        private int _inlineCount;
        private bool _isDescendant;
        private int _tagIndex;
        private HtmlTextWriterTag _tagKey;
        private string _tagName;

        static HtmlTextWriter()
        {

            // register known tags
            _tagKeyLookupTable = new Dictionary<string, HtmlTextWriterTag>((int)HtmlTextWriterTag.Xml + 1);
            _tagNameLookupArray = new TagInformation[(int)HtmlTextWriterTag.Xml + 1];

            RegisterTag(String.Empty, HtmlTextWriterTag.Unknown, TagType.Other);
            RegisterTag("a", HtmlTextWriterTag.A, TagType.Inline);
            RegisterTag("acronym", HtmlTextWriterTag.Acronym, TagType.Inline);
            RegisterTag("address", HtmlTextWriterTag.Address, TagType.Other);
            RegisterTag("area", HtmlTextWriterTag.Area, TagType.NonClosing);
            RegisterTag("b", HtmlTextWriterTag.B, TagType.Inline);
            RegisterTag("base", HtmlTextWriterTag.Base, TagType.NonClosing);
            RegisterTag("basefont", HtmlTextWriterTag.Basefont, TagType.NonClosing);
            RegisterTag("bdo", HtmlTextWriterTag.Bdo, TagType.Inline);
            RegisterTag("bgsound", HtmlTextWriterTag.Bgsound, TagType.NonClosing);
            RegisterTag("big", HtmlTextWriterTag.Big, TagType.Inline);
            RegisterTag("blockquote", HtmlTextWriterTag.Blockquote, TagType.Other);
            RegisterTag("body", HtmlTextWriterTag.Body, TagType.Other);
            RegisterTag("br", HtmlTextWriterTag.Br, TagType.NonClosing);
            RegisterTag("button", HtmlTextWriterTag.Button, TagType.Inline);
            RegisterTag("caption", HtmlTextWriterTag.Caption, TagType.Other);
            RegisterTag("center", HtmlTextWriterTag.Center, TagType.Other);
            RegisterTag("cite", HtmlTextWriterTag.Cite, TagType.Inline);
            RegisterTag("code", HtmlTextWriterTag.Code, TagType.Inline);
            RegisterTag("col", HtmlTextWriterTag.Col, TagType.NonClosing);
            RegisterTag("colgroup", HtmlTextWriterTag.Colgroup, TagType.Other);
            RegisterTag("del", HtmlTextWriterTag.Del, TagType.Inline);
            RegisterTag("dd", HtmlTextWriterTag.Dd, TagType.Inline);
            RegisterTag("dfn", HtmlTextWriterTag.Dfn, TagType.Inline);
            RegisterTag("dir", HtmlTextWriterTag.Dir, TagType.Other);
            RegisterTag("div", HtmlTextWriterTag.Div, TagType.Other);
            RegisterTag("dl", HtmlTextWriterTag.Dl, TagType.Other);
            RegisterTag("dt", HtmlTextWriterTag.Dt, TagType.Inline);
            RegisterTag("em", HtmlTextWriterTag.Em, TagType.Inline);
            RegisterTag("embed", HtmlTextWriterTag.Embed, TagType.NonClosing);
            RegisterTag("fieldset", HtmlTextWriterTag.Fieldset, TagType.Other);
            RegisterTag("font", HtmlTextWriterTag.Font, TagType.Inline);
            RegisterTag("form", HtmlTextWriterTag.Form, TagType.Other);
            RegisterTag("frame", HtmlTextWriterTag.Frame, TagType.NonClosing);
            RegisterTag("frameset", HtmlTextWriterTag.Frameset, TagType.Other);
            RegisterTag("h1", HtmlTextWriterTag.H1, TagType.Other);
            RegisterTag("h2", HtmlTextWriterTag.H2, TagType.Other);
            RegisterTag("h3", HtmlTextWriterTag.H3, TagType.Other);
            RegisterTag("h4", HtmlTextWriterTag.H4, TagType.Other);
            RegisterTag("h5", HtmlTextWriterTag.H5, TagType.Other);
            RegisterTag("h6", HtmlTextWriterTag.H6, TagType.Other);
            RegisterTag("head", HtmlTextWriterTag.Head, TagType.Other);
            RegisterTag("hr", HtmlTextWriterTag.Hr, TagType.NonClosing);
            RegisterTag("html", HtmlTextWriterTag.Html, TagType.Other);
            RegisterTag("i", HtmlTextWriterTag.I, TagType.Inline);
            RegisterTag("iframe", HtmlTextWriterTag.Iframe, TagType.Other);
            RegisterTag("img", HtmlTextWriterTag.Img, TagType.NonClosing);
            RegisterTag("input", HtmlTextWriterTag.Input, TagType.NonClosing);
            RegisterTag("ins", HtmlTextWriterTag.Ins, TagType.Inline);
            RegisterTag("isindex", HtmlTextWriterTag.Isindex, TagType.NonClosing);
            RegisterTag("kbd", HtmlTextWriterTag.Kbd, TagType.Inline);
            RegisterTag("label", HtmlTextWriterTag.Label, TagType.Inline);
            RegisterTag("legend", HtmlTextWriterTag.Legend, TagType.Other);
            RegisterTag("li", HtmlTextWriterTag.Li, TagType.Inline);
            RegisterTag("link", HtmlTextWriterTag.Link, TagType.NonClosing);
            RegisterTag("map", HtmlTextWriterTag.Map, TagType.Other);
            RegisterTag("marquee", HtmlTextWriterTag.Marquee, TagType.Other);
            RegisterTag("menu", HtmlTextWriterTag.Menu, TagType.Other);
            RegisterTag("meta", HtmlTextWriterTag.Meta, TagType.NonClosing);
            RegisterTag("nobr", HtmlTextWriterTag.Nobr, TagType.Inline);
            RegisterTag("noframes", HtmlTextWriterTag.Noframes, TagType.Other);
            RegisterTag("noscript", HtmlTextWriterTag.Noscript, TagType.Other);
            RegisterTag("object", HtmlTextWriterTag.Object, TagType.Other);
            RegisterTag("ol", HtmlTextWriterTag.Ol, TagType.Other);
            RegisterTag("option", HtmlTextWriterTag.Option, TagType.Other);
            RegisterTag("p", HtmlTextWriterTag.P, TagType.Inline);
            RegisterTag("param", HtmlTextWriterTag.Param, TagType.Other);
            RegisterTag("pre", HtmlTextWriterTag.Pre, TagType.Other);
            RegisterTag("ruby", HtmlTextWriterTag.Ruby, TagType.Other);
            RegisterTag("rt", HtmlTextWriterTag.Rt, TagType.Other);
            RegisterTag("q", HtmlTextWriterTag.Q, TagType.Inline);
            RegisterTag("s", HtmlTextWriterTag.S, TagType.Inline);
            RegisterTag("samp", HtmlTextWriterTag.Samp, TagType.Inline);
            RegisterTag("script", HtmlTextWriterTag.Script, TagType.Other);
            RegisterTag("select", HtmlTextWriterTag.Select, TagType.Other);
            RegisterTag("small", HtmlTextWriterTag.Small, TagType.Other);
            RegisterTag("span", HtmlTextWriterTag.Span, TagType.Inline);
            RegisterTag("strike", HtmlTextWriterTag.Strike, TagType.Inline);
            RegisterTag("strong", HtmlTextWriterTag.Strong, TagType.Inline);
            RegisterTag("style", HtmlTextWriterTag.Style, TagType.Other);
            RegisterTag("sub", HtmlTextWriterTag.Sub, TagType.Inline);
            RegisterTag("sup", HtmlTextWriterTag.Sup, TagType.Inline);
            RegisterTag("table", HtmlTextWriterTag.Table, TagType.Other);
            RegisterTag("tbody", HtmlTextWriterTag.Tbody, TagType.Other);
            RegisterTag("td", HtmlTextWriterTag.Td, TagType.Inline);
            RegisterTag("textarea", HtmlTextWriterTag.Textarea, TagType.Inline);
            RegisterTag("tfoot", HtmlTextWriterTag.Tfoot, TagType.Other);
            RegisterTag("th", HtmlTextWriterTag.Th, TagType.Inline);
            RegisterTag("thead", HtmlTextWriterTag.Thead, TagType.Other);
            RegisterTag("title", HtmlTextWriterTag.Title, TagType.Other);
            RegisterTag("tr", HtmlTextWriterTag.Tr, TagType.Other);
            RegisterTag("tt", HtmlTextWriterTag.Tt, TagType.Inline);
            RegisterTag("u", HtmlTextWriterTag.U, TagType.Inline);
            RegisterTag("ul", HtmlTextWriterTag.Ul, TagType.Other);
            RegisterTag("var", HtmlTextWriterTag.Var, TagType.Inline);
            RegisterTag("wbr", HtmlTextWriterTag.Wbr, TagType.NonClosing);
            RegisterTag("xml", HtmlTextWriterTag.Xml, TagType.Other);

            // register known attributes
            _attrKeyLookupTable = new Dictionary<string, HtmlTextWriterAttribute>((int)HtmlTextWriterAttribute.VCardName + 1);
            _attrNameLookupArray = new AttributeInformation[(int)HtmlTextWriterAttribute.VCardName + 1];

            RegisterAttribute("abbr", HtmlTextWriterAttribute.Abbr, true);
            RegisterAttribute("accesskey", HtmlTextWriterAttribute.Accesskey, true);
            RegisterAttribute("align", HtmlTextWriterAttribute.Align, false);
            RegisterAttribute("alt", HtmlTextWriterAttribute.Alt, true);
            RegisterAttribute("autocomplete", HtmlTextWriterAttribute.AutoComplete, false);
            RegisterAttribute("axis", HtmlTextWriterAttribute.Axis, true);
            RegisterAttribute("background", HtmlTextWriterAttribute.Background, true, true);
            RegisterAttribute("bgcolor", HtmlTextWriterAttribute.Bgcolor, false);
            RegisterAttribute("border", HtmlTextWriterAttribute.Border, false);
            RegisterAttribute("bordercolor", HtmlTextWriterAttribute.Bordercolor, false);
            RegisterAttribute("cellpadding", HtmlTextWriterAttribute.Cellpadding, false);
            RegisterAttribute("cellspacing", HtmlTextWriterAttribute.Cellspacing, false);
            RegisterAttribute("checked", HtmlTextWriterAttribute.Checked, false);
            RegisterAttribute("class", HtmlTextWriterAttribute.Class, true);
            RegisterAttribute("cols", HtmlTextWriterAttribute.Cols, false);
            RegisterAttribute("colspan", HtmlTextWriterAttribute.Colspan, false);
            RegisterAttribute("content", HtmlTextWriterAttribute.Content, true);
            RegisterAttribute("coords", HtmlTextWriterAttribute.Coords, false);
            RegisterAttribute("dir", HtmlTextWriterAttribute.Dir, false);
            RegisterAttribute("disabled", HtmlTextWriterAttribute.Disabled, false);
            RegisterAttribute("for", HtmlTextWriterAttribute.For, false);
            RegisterAttribute("headers", HtmlTextWriterAttribute.Headers, true);
            RegisterAttribute("height", HtmlTextWriterAttribute.Height, false);
            RegisterAttribute("href", HtmlTextWriterAttribute.Href, true, true);
            RegisterAttribute("id", HtmlTextWriterAttribute.Id, false);
            RegisterAttribute("longdesc", HtmlTextWriterAttribute.Longdesc, true, true);
            RegisterAttribute("maxlength", HtmlTextWriterAttribute.Maxlength, false);
            RegisterAttribute("multiple", HtmlTextWriterAttribute.Multiple, false);
            RegisterAttribute("name", HtmlTextWriterAttribute.Name, false);
            RegisterAttribute("nowrap", HtmlTextWriterAttribute.Nowrap, false);
            RegisterAttribute("onclick", HtmlTextWriterAttribute.Onclick, true);
            RegisterAttribute("onchange", HtmlTextWriterAttribute.Onchange, true);
            RegisterAttribute("readonly", HtmlTextWriterAttribute.ReadOnly, false);
            RegisterAttribute("rel", HtmlTextWriterAttribute.Rel, false);
            RegisterAttribute("rows", HtmlTextWriterAttribute.Rows, false);
            RegisterAttribute("rowspan", HtmlTextWriterAttribute.Rowspan, false);
            RegisterAttribute("rules", HtmlTextWriterAttribute.Rules, false);
            RegisterAttribute("scope", HtmlTextWriterAttribute.Scope, false);
            RegisterAttribute("selected", HtmlTextWriterAttribute.Selected, false);
            RegisterAttribute("shape", HtmlTextWriterAttribute.Shape, false);
            RegisterAttribute("size", HtmlTextWriterAttribute.Size, false);
            RegisterAttribute("src", HtmlTextWriterAttribute.Src, true, true);
            RegisterAttribute("style", HtmlTextWriterAttribute.Style, false);
            RegisterAttribute("tabindex", HtmlTextWriterAttribute.Tabindex, false);
            RegisterAttribute("target", HtmlTextWriterAttribute.Target, false);
            RegisterAttribute("title", HtmlTextWriterAttribute.Title, true);
            RegisterAttribute("type", HtmlTextWriterAttribute.Type, false);
            RegisterAttribute("usemap", HtmlTextWriterAttribute.Usemap, false);
            RegisterAttribute("valign", HtmlTextWriterAttribute.Valign, false);
            RegisterAttribute("value", HtmlTextWriterAttribute.Value, true);
            RegisterAttribute("vcard_name", HtmlTextWriterAttribute.VCardName, false);
            RegisterAttribute("width", HtmlTextWriterAttribute.Width, false);
            RegisterAttribute("wrap", HtmlTextWriterAttribute.Wrap, false);
        }

        public override Encoding Encoding
        {
            get
            {
                return writer.Encoding;
            }
        }

#if NETSTANDARD1_5
        public HtmlEncoder Encoder { get; set; } = HtmlEncoder.Default;
#endif

        // Gets or sets the new line character to use.
        public override string NewLine
        {
            get
            {
                return writer.NewLine;
            }

            set
            {
                writer.NewLine = value;
            }
        }

        // Gets or sets the number of spaces to indent.
        public int Indent
        {
            get
            {
                return indentLevel;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                indentLevel = value;
            }
        }

        //Gets or sets the TextWriter to use.
        public TextWriter InnerWriter
        {
            get
            {
                return writer;
            }
            set
            {
                writer = value;
            }
        }

        public override void Flush()
        {
            writer.Flush();
        }

        protected virtual void OutputTabs()
        {
            if (tabsPending)
            {
                for (int i = 0; i < indentLevel; i++)
                {
                    writer.Write(tabString);
                }
                tabsPending = false;
            }
        }

        //Writes a string to the text stream.
        public override void Write(string s)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(s);
        }

        //Writes the text representation of a Boolean value to the text stream.
        public override void Write(bool value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        //Writes a character to the text stream.
        public override void Write(char value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        // Writes a character array to the text stream.
        public override void Write(char[] buffer)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(buffer);
        }

        // Writes a subarray of characters to the text stream.
        public override void Write(char[] buffer, int index, int count)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(buffer, index, count);
        }

        // Writes the text representation of a Double to the text stream.
        public override void Write(double value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        // Writes the text representation of a Single to the text stream.
        public override void Write(float value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        // Writes the text representation of an integer to the text stream.
        public override void Write(int value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        // Writes the text representation of an 8-byte integer to the text stream.
        public override void Write(long value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        // Writes the text representation of an object to the text stream.
        public override void Write(object value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(value);
        }

        // Writes out a formatted string, using the same semantics as specified.
        public override void Write(string format, object arg0)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(format, arg0);
        }

        // Writes out a formatted string, using the same semantics as specified.
        public override void Write(string format, object arg0, object arg1)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(format, arg0, arg1);
        }

        // Writes out a formatted string, using the same semantics as specified.
        public override void Write(string format, params object[] arg)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(format, arg);
        }

        // Writes the specified string to a line without tabs.
        public void WriteLineNoTabs(string s)
        {
            writer.WriteLine(s);
            tabsPending = true;
        }

        // Writes the specified string followed by a line terminator to the text stream.
        public override void WriteLine(string s)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(s);
            tabsPending = true;
        }

        // Writes a line terminator.
        public override void WriteLine()
        {
            writer.WriteLine();
            tabsPending = true;
        }

        // Writes the text representation of a Boolean followed by a line terminator to the text stream.
        public override void WriteLine(bool value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(char value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(char[] buffer)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(buffer);
            tabsPending = true;
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(buffer, index, count);
            tabsPending = true;
        }

        public override void WriteLine(double value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(float value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(int value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(long value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(object value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        public override void WriteLine(string format, object arg0)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(format, arg0);
            tabsPending = true;
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(format, arg0, arg1);
            tabsPending = true;
        }

        public override void WriteLine(string format, params object[] arg)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(format, arg);
            tabsPending = true;
        }

        public override void WriteLine(UInt32 value)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.WriteLine(value);
            tabsPending = true;
        }

        protected static void RegisterTag(string name, HtmlTextWriterTag key)
        {
            RegisterTag(name, key, TagType.Other);
        }

        private static void RegisterTag(string name, HtmlTextWriterTag key, TagType type)
        {
            string nameLCase = name.ToLower();

            _tagKeyLookupTable.Add(nameLCase, key);

            // Pre-resolve the end tag
            string endTag = null;
            if (type != TagType.NonClosing && key != HtmlTextWriterTag.Unknown)
            {
                endTag = EndTagLeftChars + nameLCase + TagRightChar;
            }

            if ((int)key < _tagNameLookupArray.Length)
            {
                _tagNameLookupArray[(int)key] = new TagInformation(name, type, endTag);
            }
        }

        protected static void RegisterAttribute(string name, HtmlTextWriterAttribute key)
        {
            RegisterAttribute(name, key, false);
        }

        private static void RegisterAttribute(string name, HtmlTextWriterAttribute key, bool encode)
        {
            RegisterAttribute(name, key, encode, false);
        }

        private static void RegisterAttribute(string name, HtmlTextWriterAttribute key, bool encode, bool isUrl)
        {
            string nameLCase = name.ToLower();

            _attrKeyLookupTable.Add(nameLCase, key);

            if ((int)key < _attrNameLookupArray.Length)
            {
                _attrNameLookupArray[(int)key] = new AttributeInformation(name, encode, isUrl);
            }
        }

        public HtmlTextWriter(TextWriter writer) : this(writer, DefaultTabString)
        {
        }

        public HtmlTextWriter(TextWriter writer, string tabString) : base(CultureInfo.InvariantCulture)
        {
            this.writer = writer;
            this.tabString = tabString;
            indentLevel = 0;
            tabsPending = false;

            _isDescendant = (GetType() != typeof(HtmlTextWriter));

            _attrCount = 0;
            _endTagCount = 0;
            _inlineCount = 0;
        }

        protected HtmlTextWriterTag TagKey
        {
            get
            {
                return _tagKey;
            }
            set
            {
                _tagIndex = (int)value;
                if (_tagIndex < 0 || _tagIndex >= _tagNameLookupArray.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _tagKey = value;
                // If explicitly setting to uknown, keep the old tag name. This allows a string tag
                // to be set without clobbering it if setting TagKey to itself.
                if (value != HtmlTextWriterTag.Unknown)
                {
                    _tagName = _tagNameLookupArray[_tagIndex].name;
                }
            }
        }

        protected string TagName
        {
            get
            {
                return _tagName;
            }
            set
            {
                _tagName = value;
                _tagKey = GetTagKey(_tagName);
                _tagIndex = (int)_tagKey;
            }
        }

        public virtual void AddAttribute(string name, string value)
        {
            HtmlTextWriterAttribute attributeKey = GetAttributeKey(name);
            value = EncodeAttributeValue(attributeKey, value);

            AddAttribute(name, value, attributeKey);
        }

        //do not fix this spelling error
        //believe it or not, it is a backwards breaking change for languages that 
        //support late binding with named parameters VB.Net
        public virtual void AddAttribute(string name, string value, bool fEndode)
        {
            value = EncodeAttributeValue(value, fEndode);
            AddAttribute(name, value, GetAttributeKey(name));
        }

        public virtual void AddAttribute(HtmlTextWriterAttribute key, string value)
        {
            int attributeIndex = (int)key;
            if (attributeIndex >= 0 && attributeIndex < _attrNameLookupArray.Length)
            {
                AttributeInformation info = _attrNameLookupArray[attributeIndex];
                AddAttribute(info.name, value, key, info.encode, info.isUrl);
            }
        }

        public virtual void AddAttribute(HtmlTextWriterAttribute key, string value, bool fEncode)
        {
            int attributeIndex = (int)key;
            if (attributeIndex >= 0 && attributeIndex < _attrNameLookupArray.Length)
            {
                AttributeInformation info = _attrNameLookupArray[attributeIndex];
                AddAttribute(info.name, value, key, fEncode, info.isUrl);
            }
        }

        protected virtual void AddAttribute(string name, string value, HtmlTextWriterAttribute key)
        {
            AddAttribute(name, value, key, false, false);
        }


        private void AddAttribute(string name, string value, HtmlTextWriterAttribute key, bool encode, bool isUrl)
        {
            if (_attrList == null)
            {
                _attrList = new RenderAttribute[20];
            }
            else if (_attrCount >= _attrList.Length)
            {
                RenderAttribute[] newArray = new RenderAttribute[_attrList.Length * 2];
                Array.Copy(_attrList, newArray, _attrList.Length);
                _attrList = newArray;
            }
            RenderAttribute attr;
            attr.name = name;
            attr.value = value;
            attr.key = key;
            attr.encode = encode;
            attr.isUrl = isUrl;
            _attrList[_attrCount] = attr;
            _attrCount++;
        }

        protected string EncodeAttributeValue(string value, bool fEncode)
        {
            if (value == null)
            {
                return null;
            }

            if (!fEncode)
                return value;

#if NETSTANDARD1_5
            return Encoder.Encode(value);
#else
            return HttpUtility.HtmlAttributeEncode(value);
#endif
        }

        protected virtual string EncodeAttributeValue(HtmlTextWriterAttribute attrKey, string value)
        {
            bool encode = true;

            if (0 <= (int)attrKey && (int)attrKey < _attrNameLookupArray.Length)
            {
                encode = _attrNameLookupArray[(int)attrKey].encode;
            }

            return EncodeAttributeValue(value, encode);
        }

        // This does minimal URL encoding by converting spaces in the url to "%20".
        protected string EncodeUrl(string url)
        {
#if NETSTANDARD1_5
            return UrlEncoder.Default.Encode(url);
#else
            return HttpUtility.UrlPathEncode(url);
#endif
        }

        protected HtmlTextWriterAttribute GetAttributeKey(string attrName)
        {
            if (!String.IsNullOrEmpty(attrName))
            {
                HtmlTextWriterAttribute key;
                if (_attrKeyLookupTable.TryGetValue(attrName.ToLower(), out key))
                    return key;
            }

            return (HtmlTextWriterAttribute)(-1);
        }

        protected string GetAttributeName(HtmlTextWriterAttribute attrKey)
        {
            if ((int)attrKey >= 0 && (int)attrKey < _attrNameLookupArray.Length)
                return _attrNameLookupArray[(int)attrKey].name;

            return string.Empty;
        }

        protected virtual HtmlTextWriterTag GetTagKey(string tagName)
        {
            if (!String.IsNullOrEmpty(tagName))
            {
                object key = _tagKeyLookupTable[tagName.ToLower()];
                if (key != null)
                    return (HtmlTextWriterTag)key;
            }

            return HtmlTextWriterTag.Unknown;
        }

        protected virtual string GetTagName(HtmlTextWriterTag tagKey)
        {
            int tagIndex = (int)tagKey;
            if (tagIndex >= 0 && tagIndex < _tagNameLookupArray.Length)
                return _tagNameLookupArray[tagIndex].name;

            return string.Empty;
        }

        protected bool IsAttributeDefined(HtmlTextWriterAttribute key)
        {
            for (int i = 0; i < _attrCount; i++)
            {
                if (_attrList[i].key == key)
                {
                    return true;
                }
            }
            return false;
        }

        protected bool IsAttributeDefined(HtmlTextWriterAttribute key, out string value)
        {
            value = null;
            for (int i = 0; i < _attrCount; i++)
            {
                if (_attrList[i].key == key)
                {
                    value = _attrList[i].value;
                    return true;
                }
            }
            return false;
        }

        protected virtual bool OnAttributeRender(string name, string value, HtmlTextWriterAttribute key)
        {
            return true;
        }

        protected virtual bool OnTagRender(string name, HtmlTextWriterTag key)
        {
            return true;
        }

        protected string PopEndTag()
        {
            if (_endTagCount <= 0)
            {
                throw new InvalidOperationException("Unbalanced end tag pop.");
            }
            _endTagCount--;
            TagKey = _endTags[_endTagCount].tagKey;
            return _endTags[_endTagCount].endTagText;
        }

        protected void PushEndTag(string endTag)
        {
            if (_endTags == null)
            {
                _endTags = new TagStackEntry[16];
            }
            else if (_endTagCount >= _endTags.Length)
            {
                TagStackEntry[] newArray = new TagStackEntry[_endTags.Length * 2];
                Array.Copy(_endTags, newArray, _endTags.Length);
                _endTags = newArray;
            }
            _endTags[_endTagCount].tagKey = _tagKey;
            _endTags[_endTagCount].endTagText = endTag;
            _endTagCount++;
        }

        // This calls filers out all attributes and style attributes by calling OnAttributeRender
        // and OnStyleAttributeRender on all properites and updates the lists</para>
        protected virtual void FilterAttributes()
        {
            // Create the filtered list of attributes
            int newAttrCount = 0;
            for (int i = 0; i < _attrCount; i++)
            {
                RenderAttribute attr = _attrList[i];
                if (OnAttributeRender(attr.name, attr.value, attr.key))
                {
                    // Update the list. This can be done in place
                    _attrList[newAttrCount] = attr;
                    newAttrCount++;
                }
            }
            // Update the count
            _attrCount = newAttrCount;
        }

        public virtual void RenderBeginTag(string tagName)
        {
            this.TagName = tagName;
            RenderBeginTag(_tagKey);
        }

        public virtual void RenderBeginTag(HtmlTextWriterTag tagKey)
        {

            this.TagKey = tagKey;
            bool renderTag = true;

            if (_isDescendant)
            {
                renderTag = OnTagRender(_tagName, _tagKey);

                // Inherited renderers will be expecting to be able to filter any of the attributes at this point
                FilterAttributes();

                // write text before begin tag
                string textBeforeTag = RenderBeforeTag();
                if (textBeforeTag != null)
                {
                    if (tabsPending)
                    {
                        OutputTabs();
                    }
                    writer.Write(textBeforeTag);
                }
            }

            // gather information about this tag.
            TagInformation tagInfo = _tagNameLookupArray[_tagIndex];
            TagType tagType = tagInfo.tagType;
            bool renderEndTag = renderTag && (tagType != TagType.NonClosing);
            string endTag = renderEndTag ? tagInfo.closingTag : null;

            // write the begin tag
            if (renderTag)
            {
                if (tabsPending)
                {
                    OutputTabs();
                }
                writer.Write(TagLeftChar);
                writer.Write(_tagName);

                for (int i = 0; i < _attrCount; i++)
                {
                    RenderAttribute attr = _attrList[i];
                    writer.Write(SpaceChar);
                    writer.Write(attr.name);
                    if (attr.value != null)
                    {
                        writer.Write(EqualsDoubleQuoteString);

                        string attrValue = attr.value;
                        if (attr.isUrl)
                        {
                            if (attr.key != HtmlTextWriterAttribute.Href || !attrValue.StartsWith("javascript:", StringComparison.Ordinal))
                            {
                                attrValue = EncodeUrl(attrValue);
                            }
                        }
                        if (attr.encode)
                        {
                            WriteHtmlAttributeEncode(attrValue);
                        }
                        else {
                            writer.Write(attrValue);
                        }
                        writer.Write(DoubleQuoteChar);
                    }
                }


                if (tagType == TagType.NonClosing)
                {
                    writer.Write(SelfClosingTagEnd);
                }
                else {
                    writer.Write(TagRightChar);
                }
            }

            string textBeforeContent = RenderBeforeContent();
            if (textBeforeContent != null)
            {
                if (tabsPending)
                {
                    OutputTabs();
                }
                writer.Write(textBeforeContent);
            }

            // write text before the content
            if (renderEndTag)
            {

                if (tagType == TagType.Inline)
                {
                    _inlineCount += 1;
                }
                else {
                    // writeline and indent before rendering content
                    WriteLine();
                    Indent++;
                }
                // Manually build end tags for unknown tag types.
                if (endTag == null)
                {
                    endTag = EndTagLeftChars + _tagName + TagRightChar.ToString();
                }
            }

            if (_isDescendant)
            {
                // append text after the tag
                string textAfterTag = RenderAfterTag();
                if (textAfterTag != null)
                {
                    endTag = (endTag == null) ? textAfterTag : textAfterTag + endTag;
                }

                // build end content and push it on stack to write in RenderEndTag
                // prepend text after the content
                string textAfterContent = RenderAfterContent();
                if (textAfterContent != null)
                {
                    endTag = (endTag == null) ? textAfterContent : textAfterContent + endTag;
                }
            }

            // push end tag onto stack
            PushEndTag(endTag);

            // flush attribute and style lists for next tag
            _attrCount = 0;

        }

        public virtual void RenderEndTag()
        {
            string endTag = PopEndTag();

            if (endTag != null)
            {
                if (_tagNameLookupArray[_tagIndex].tagType == TagType.Inline)
                {
                    _inlineCount -= 1;
                    // Never inject crlfs at end of inline tags.
                    //
                    Write(endTag);
                }
                else {
                    // unindent if not an inline tag
                    WriteLine();
                    this.Indent--;
                    Write(endTag);
                }
            }
        }

        protected virtual string RenderBeforeTag()
        {
            return null;
        }

        protected virtual string RenderBeforeContent()
        {
            return null;
        }

        protected virtual string RenderAfterContent()
        {
            return null;
        }

        protected virtual string RenderAfterTag()
        {
            return null;
        }

        public virtual void WriteAttribute(string name, string value)
        {
            WriteAttribute(name, value, false /*encode*/);
        }

        public virtual void WriteAttribute(string name, string value, bool fEncode)
        {
            writer.Write(SpaceChar);
            writer.Write(name);
            if (value != null)
            {
                writer.Write(EqualsDoubleQuoteString);
                if (fEncode)
                {
                    WriteHtmlAttributeEncode(value);
                }
                else {
                    writer.Write(value);
                }
                writer.Write(DoubleQuoteChar);
            }
        }

        public virtual void WriteBeginTag(string tagName)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(TagLeftChar);
            writer.Write(tagName);
        }

        public virtual void WriteBreak()
        {
            // Space between br and / is for improved html compatibility.  See XHTML 1.0 specification, section C.2.
            Write("<br />");
        }

        // DevDiv 33149: A backward compat. switch for Everett rendering
        internal void WriteObsoleteBreak()
        {
            Write("<br>");
        }

        public virtual void WriteFullBeginTag(string tagName)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(TagLeftChar);
            writer.Write(tagName);
            writer.Write(TagRightChar);
        }

        public virtual void WriteEndTag(string tagName)
        {
            if (tabsPending)
            {
                OutputTabs();
            }
            writer.Write(TagLeftChar);
            writer.Write(SlashChar);
            writer.Write(tagName);
            writer.Write(TagRightChar);
        }

        public virtual void WriteStyleAttribute(string name, string value)
        {
            WriteStyleAttribute(name, value, false /*encode*/);
        }

        public virtual void WriteStyleAttribute(string name, string value, bool fEncode)
        {
            writer.Write(name);
            writer.Write(StyleEqualsChar);
            if (fEncode)
            {
                WriteHtmlAttributeEncode(value);
            }
            else {
                writer.Write(value);
            }
            writer.Write(SemicolonChar);
        }


        public virtual void WriteEncodedText(String text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            const char NBSP = '\u00A0';

            // When inner text is retrieved for a text control, &nbsp; is
            // decoded to 0x00A0 (code point for nbsp in Unicode).
            // HtmlEncode doesn't encode 0x00A0  to &nbsp;, we need to do it
            // manually here.
            int length = text.Length;
            int pos = 0;
            while (pos < length)
            {
                int nbsp = text.IndexOf(NBSP, pos);
                if (nbsp < 0)
                {
                    var value = pos == 0 ? text : text.Substring(pos, length - pos);
#if NETSTANDARD1_5
                    Encoder.Encode(this, value);
#else
                    HttpUtility.HtmlEncode(value, this);
#endif
                    pos = length;
                }
                else {
                    if (nbsp > pos)
                    {
                        var value = text.Substring(pos, nbsp - pos);
#if NETSTANDARD1_5
                        Encoder.Encode(this, value);
#else
                        HttpUtility.HtmlEncode(text.Substring(pos, nbsp - pos), this);
#endif
                    }
                    Write("&nbsp;");
                    pos = nbsp + 1;
                }
            }
        }

        internal void WriteHtmlAttributeEncode(string s)
        {
#if NETSTANDARD1_5
            Encoder.Encode(writer, s);
#else
            HttpUtility.HtmlAttributeEncode(s, writer);
#endif
        }

        private struct TagStackEntry
        {
            public HtmlTextWriterTag tagKey;
            public string endTagText;
        }

        private struct RenderAttribute
        {
            public string name;
            public string value;
            public HtmlTextWriterAttribute key;
            public bool encode;
            public bool isUrl;
        }

        private struct AttributeInformation
        {
            public string name;
            public bool isUrl;
            public bool encode;

            public AttributeInformation(string name, bool encode, bool isUrl)
            {
                this.name = name;
                this.encode = encode;
                this.isUrl = isUrl;
            }
        }

        private enum TagType
        {
            Inline,
            NonClosing,
            Other,
        }

        private struct TagInformation
        {
            public string name;
            public TagType tagType;
            public string closingTag;

            public TagInformation(string name, TagType tagType, string closingTag)
            {
                this.name = name;
                this.tagType = tagType;
                this.closingTag = closingTag;
            }
        }
    }
}

