namespace HtmlTags.Conventions
{
    /// <summary>
    /// Will always need to implement an Equals and GetHashCode in order for this thing to work
    /// </summary>
    public abstract class TagRequest
    {
        private HtmlTag _currentTag;
        private HtmlTag _originalTag;

        public HtmlTag OriginalTag
        {
            get { return _originalTag; }
        }

        public HtmlTag CurrentTag
        {
            get { return _currentTag; }
        }

        public void WrapWith(HtmlTag tag)
        {
            _currentTag.WrapWith(tag);
            ReplaceTag(tag);
        }

        public void ReplaceTag(HtmlTag tag)
        {
            if (_originalTag == null)
            {
                _originalTag = tag;
            }

            _currentTag = tag;
        }

        public abstract object ToToken();
    }
}