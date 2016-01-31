namespace HtmlTags.Conventions
{
    public class TagSubject
    {
        public TagSubject(string profile, ElementRequest subject)
        {
            Profile = profile ?? TagConstants.Default;
            Subject = subject;
        }

        public string Profile { get; }

        public ElementRequest Subject { get; }

        public bool Equals(TagSubject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Profile, Profile) && Equals(other.Subject, Subject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TagSubject)) return false;
            return Equals((TagSubject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Profile?.GetHashCode() ?? 0)*397) ^
                       (Subject?.GetHashCode() ?? 0);
            }
        }

        public override string ToString() => $"Profile: {Profile}, Subject: {Subject}";
    }
}