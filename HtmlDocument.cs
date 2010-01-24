using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace HtmlTags
{
    public class HtmlDocument
    {
        private readonly List<Func<string, string>> _alterations = new List<Func<string, string>>();
        private readonly HtmlTag _body;

        private readonly Stack<HtmlTag> _currentStack = new Stack<HtmlTag>();
        private readonly HtmlTag _head;
        private readonly HtmlTag _title;
        private readonly HtmlTag _top = new HtmlTag("html").Attr("xmlns", "http://www.w3.org/1999/xhtml");
        private HtmlTag _last;

        public HtmlDocument()
        {
            DocType =
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">";
            _head = _top.Add("head");
            _title = _head.Add("title");
            _body = _top.Add("body");
            _last = _body;
        }

        public string DocType { get; set; }
        public string Title { get { return _title.Text(); } set { _title.Text(value); } }
        public HtmlTag Current { get { return _currentStack.Count == 0 ? _body : _currentStack.Peek(); } }
        public HtmlTag Last { get { return _last; } }

        public void WriteToFile(string fileName)
        {
            ensureFolderExists(fileName);

            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(ToString());
            }
        }


        public void OpenInBrowser()
        {
            string filename = getPath();
            WriteToFile(filename);

            Process.Start(filename);
        }

        protected virtual string getPath()
        {
            return Path.GetTempFileName() + ".htm";
        }

        private void ensureFolderExists(string fileName)
        {
            string folder = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        public HtmlTag Add(string tagName)
        {
            _last = Current.Add(tagName);
            return _last;
        }

        public void Add(HtmlTag tag)
        {
            _last = tag;
            Current.Child(tag);
        }

        public HtmlTag Push(string tagName)
        {
            HtmlTag tag = Add(tagName);
            _currentStack.Push(tag);

            return tag;
        }

        public void Pop()
        {
            if (_currentStack.Count > 0)
            {
                _currentStack.Pop();
            }
        }

        public string ToCompacted()
        {
            string returnValue = DocType + Environment.NewLine + _top.ToCompacted();
            return substitute(returnValue);
        }

        private string substitute(string value)
        {
            foreach (var alteration in _alterations)
            {
                value = alteration(value);
            }

            return value;
        }

        public override string ToString()
        {
            string value = DocType + Environment.NewLine + _top.ToString();
            return substitute(value);
        }

        public void AddStyle(string styling)
        {
            string key = Guid.NewGuid().ToString();
            _head.Add("style").Text(key);

            _alterations.Add(html => html.Replace(key, styling));
        }

        public void AddJavaScript(string javascript)
        {
            string key = Guid.NewGuid().ToString();
            _head.Add("script").Attr("type", "text/javascript").Text(key);

            _alterations.Add(html => html.Replace(key, "\n" + javascript + "\n"));
        }

        public void ReferenceJavaScriptFile(string path)
        {
            _head.Add("script").Attr("type", "text/javascript").Attr("language", "javascript").Attr("src", path);
        }

        public void ReferenceStyle(string path)
        {
            _head.Add("link")
                .Attr("media", "screen")
                .Attr("href", path)
                .Attr("type", "text/css")
                .Attr("rel", "stylesheet");
        }
    }
}