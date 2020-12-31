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

        private BuilderSet BuildersFor(string category) => Library.TagLibrary.Category(category).Profile(_profileName);

        public ElementCategoryExpression Labels => new(BuildersFor(ElementConstants.Label));

        public ElementCategoryExpression Displays => new(BuildersFor(ElementConstants.Display));

        public ElementCategoryExpression Editors => new(BuildersFor(ElementConstants.Editor));

        public ElementCategoryExpression ValidationMessages => new(BuildersFor(ElementConstants.ValidationMessage));

        public void Apply(HtmlConventionLibrary library) => library.Import(Library);
    }
}