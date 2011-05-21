using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace HtmlTags
{
    public class HtmlDocument
    {
        private readonly List<Func<string, string>> _alterations = new List<Func<string, string>>();
        private readonly HtmlTag _body;

        private readonly Stack<HtmlTag> _currentStack = new Stack<HtmlTag>();
        private readonly HtmlTag _head;
        private readonly HtmlTag _title;
        private readonly HtmlTag _top = new HtmlTag("html");

        public HtmlDocument()
        {
            DocType = "<!DOCTYPE html>";
            _head = _top.Add("head");
            _title = _head.Add("title");
            _body = _top.Add("body");
            Last = _body;
        }

        public string DocType { get; set; }
        public HtmlTag Html { get { return _top; } }
        public string Title { get { return _title.Text(); } set { _title.Text(value); } }
        public HtmlTag Current { get { return _currentStack.Any() ? _currentStack.Peek() : _body; } }
        public HtmlTag Last { get; private set; }
        public Action<string, string> FileWriter = writeToFile;
        public Action<string> FileOpener = openFile;

        public void WriteToFile(string fileName)
        {
            FileWriter(fileName, ToString());
        }

        public void OpenInBrowser()
        {
            var filename = getPath();
            WriteToFile(filename);
            FileOpener(filename);
        }

        public HtmlTag Body
        {
            get { return _body; }
        }

        public HtmlTag Head
        {
            get { return _head; }
        }

        protected virtual string getPath()
        {
            return Path.GetTempFileName() + ".htm";
        }

        private static void writeToFile(string fileName, string fileContents)
        {
            ensureFolderExists(fileName);

            using (var writer = new StreamWriter(fileName))
            {
                writer.WriteLine(fileContents);
            }
        }

        private static void ensureFolderExists(string fileName)
        {
            var folder = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(folder))
            {
                return;
            }

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static void openFile(string fileName)
        {
            Process.Start(fileName);
        }

        public HtmlTag Add(string tagName)
        {
            Last = Current.Add(tagName);
            return Last;
        }

        public void Add(HtmlTag tag)
        {
            Last = tag;
            Current.Append(tag);
        }

        public void Add(ITagSource source)
        {
            source.AllTags().Each(Add);
        }

        public HtmlTag Push(string tagName)
        {
            var tag = Add(tagName);
            _currentStack.Push(tag);

            return tag;
        }

        public void Push(HtmlTag tag)
        {
            Current.Append(tag);
            _currentStack.Push(tag);
        }

        public void PushWithoutAttaching(HtmlTag tag)
        {
            _currentStack.Push(tag);
        }

        public void Pop()
        {
            if (_currentStack.Any())
            {
                _currentStack.Pop();
            }
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
            var value = DocType + Environment.NewLine + _top;
            return substitute(value);
        }

        public void AddStyle(string styling)
        {
            var key = Guid.NewGuid().ToString();
            _head.Add("style").Text(key);

            _alterations.Add(html => html.Replace(key, styling));
        }

        public void AddJavaScript(string javascript)
        {
            var key = Guid.NewGuid().ToString();
            _head.Add("script").Attr("type", "text/javascript").Text(key);

            _alterations.Add(html => html.Replace(key, Environment.NewLine + javascript + Environment.NewLine));
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