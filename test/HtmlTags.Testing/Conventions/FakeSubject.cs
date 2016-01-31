using HtmlTags.Conventions;

namespace HtmlTags.Testing.Conventions
{
    using Reflection;

    public class ByNameBuilder : ITagBuilder
    {
        public bool Matches(ElementRequest subject)
        {
            return true;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").Id(((FakeSubject)request).Name);
        }
    }


    public class FakeBuilder : ITagBuilder
    {
        private readonly int _level;
        private readonly string _id;

        public FakeBuilder(int level, string id)
        {
            _level = level;
            _id = id;
        }

        public bool Matches(FakeSubject subject)
        {
            return subject.Level == _level;
        }

        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("div").Id(_id);
        }
    }

    public class FakeAddClass : ITagModifier
    {
        private readonly int _level;
        private readonly string _class;

        public FakeAddClass(int level, string @class)
        {
            _level = level;
            _class = @class;
        }

        public bool Matches(ElementRequest token)
        {
            return ((FakeSubject)token).Level <= _level;
        }

        public void Modify(ElementRequest request)
        {
            request.CurrentTag.AddClass(_class);
        }
    }

    public class FakeSubject : ElementRequest
    {
        public FakeSubject()
            : base(SingleProperty.Build<FakeSubject>(m => m.ElementId))
        {
        }

        public string Name { get; set; }
        public int Level { get; set; }
        public string[] Items { get; set; }

        public bool Equals(FakeSubject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && other.Level == Level;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FakeSubject)) return false;
            return Equals((FakeSubject) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (Name != null ? Name.GetHashCode() : 0);
                result = (result*397) ^ Level;
                result = (result*397) ^ (Items != null ? Items.GetHashCode() : 0);
                return result;
            }
        }
    }
}