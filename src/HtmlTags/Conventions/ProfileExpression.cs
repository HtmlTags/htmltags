namespace HtmlTags.Conventions
{
    using Elements;

    public class ProfileExpression
    {
        protected HtmlConventionLibrary Library { get; set; }
        private readonly string _profileName;

        public ProfileExpression(HtmlConventionLibrary library, string profileName)
        {
            Library = library;
            _profileName = profileName;
        }

        private BuilderSet buildersFor(string category)
        {
            return Library.For().Category(category).Profile(_profileName);
        }

        public ElementCategoryExpression Labels
        {
            get { return new ElementCategoryExpression(buildersFor(ElementConstants.Label)); }
        }

        public ElementCategoryExpression Displays
        {
            get { return new ElementCategoryExpression(buildersFor(ElementConstants.Display)); }
        }

        public ElementCategoryExpression Editors
        {
            get { return new ElementCategoryExpression(buildersFor(ElementConstants.Editor)); }
        }

        public void Apply(HtmlConventionLibrary library)
        {
            library.Import(Library);
        }
    }
}