### .NET objects for generating HTML

Install via [nuget](http://www.nuget.org/):

    PM> Install-Package HtmlTags

Create a `span` tag with a CSS classes and an ID:

    new HtmlTag("span").Id("first-name").AddClass("important").Text("Luke").ToString()

This will produce:

    <span id="first-name" class="important">Luke</span>