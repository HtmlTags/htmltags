#HtmlTags

 .NET objects for generating HTML

## Installation
Install via [nuget](http://www.nuget.org/):

    PM> Install-Package HtmlTags

## When should I use HtmlTags?

In general, you should avoid building strings of HTML in your applications. There are plenty of template/view engines that are much more suitable for generating dynamic markup. However, there are some situations that require you to build snippets of HTML from code (e.g., view extensions in FubuMVC or HtmlHelper extensions in ASP.NET MVC). HtmlTags is the best way to build those snippets.

### The HtmlTags advantage

* Automatic encoding of HTML entities in your attributes and inner text
* Ensures proper structure (closing tags, etc)
* Easy to use chaining API with method names modeled after jQuery
* Compatible with ASP.NET 4's encoded code expressions `<%: %>`
* Methods manipulate an internal model, not a string. The HTML string is not
generated until ToString() is called.

That last point seems trivial, but it is actually HtmlTag's biggest strength. If your HtmlHelper extensions return an HtmlTag instance, you have the ability to customize the generated tag within your view template. When your HtmlHelpers return a strings (as the helpers included with ASP.NET MVC do), you have no chance to customize the tag (without ugly string manipulation).

### HtmlHelpers that return strings

Consider the built-in ASP.NET MVC TextBox() helper:

    <%: Html.TextBox("FirstName") %>

It will generate the following HTML:

    <input id="FirstName" name="FirstName" type="text" value="" />

What if you wanted to apply a CSS class to that input? Or change the element id? Good luck.

### HtmlHelpers that return HtmlTags

Now consider an HtmlHelper extension that returns an HtmlTag:

    public static class HtmlTagHelpers {
        public static HtmlTag TextBoxTag(this HtmlHelper html, string name) {
            return new HtmlTag("input").Id(name).Attr("name", name)
                .Attr("type", "text").Attr("value", html.ViewData[name]);
        }
    }

You can use it just as easily in your view template:

    <%: Html.TextBoxTag("FirstName") %>

and it will generate the exact same HTML as above. However, you can also very easily modify the tag produced by the helper:

    <%: Html.TextBoxTag("FirstName").Id("first-name").AddClass("required") %>

will generate:

    <input id="first-name" name="FirstName" type="text" value="Lucas" class="required" />



## Usage

### Overview

Each HtmlTag instance represents a single HTML element, and optionally, a collection of inner elements.

To create an element, pass the element name to the HtmlTag constructor:

    new HtmlTag("span")

Calling ToString() on this instance will produce:

    <span></span>

There are a number of methods you can use to modify the element:

    var tag = new HtmlTag("span");
    tag.Text("Hello & Goodbye");
    tag.AddClass("important");
    tag.Attr("title", "Greeting");
    
which will produce:

    <span title="Greetings" class="important">Hello &amp; Goodbye</span>

All of the modifying methods return the instance, so they can be chained. The above example can be rewritten as:

    new HtmlTag("span").Text("Hello & Goodbye").AddClass("important").Attr("title", "Greetings")
    
Most methods that set a value also have an overload that returns the value:

	var tag = new HtmlTag("span").Attr("title", "My Tooltip");
	Console.WriteLine( tag.Attr("title") ); // writes: My Tooltip

### More information

More detailed documention is coming. In the meantime, try browsing the [unit tests](https://github.com/DarthFubuMVC/htmltags/blob/master/src/HtmlTags.Testing/HtmlTagTester.cs) for further examples.
