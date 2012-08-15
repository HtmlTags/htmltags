namespace HtmlTags.Conventions
{
    public class TagSubject<T> where T : TagRequest
    {
        private readonly string _profile;
        private readonly T _subject;

        public TagSubject(string profile, T subject)
        {
            _profile = profile ?? TagConstants.Default;
            _subject = subject;
        }

        public string Profile
        {
            get { return _profile; }
        }

        public T Subject
        {
            get { return _subject; }
        }

        public bool Equals(TagSubject<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._profile, _profile) && Equals(other._subject, _subject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TagSubject<T>)) return false;
            return Equals((TagSubject<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_profile != null ? _profile.GetHashCode() : 0)*397) ^
                       (_subject != null ? _subject.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Profile: {0}, Subject: {1}", _profile, _subject);
        }
    }
}