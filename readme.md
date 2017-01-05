# ![HtmlTags](https://raw.githubusercontent.com/HtmlTags/htmltags/master/logo/FubuHtml_32.png) HtmlTags 

[![Build status](https://ci.appveyor.com/api/projects/status/7h0p16ld5dqikglj?svg=true)](https://ci.appveyor.com/project/jbogard/htmltags)
[![NuGet Version](http://img.shields.io/nuget/v/HtmlTags.svg?style=flat)](https://www.nuget.org/packages/HtmlTags/)
[![ASP.NET Core NuGet Version](http://img.shields.io/nuget/v/HtmlTags.svg?style=flat)](https://www.nuget.org/packages/HtmlTags.AspNetCore/)
[![MyGet CI Version](https://img.shields.io/myget/htmltags-ci/v/HtmlTags.svg)](http://myget.org/gallery/htmltags-ci)

 .NET objects for generating HTML

## Installation
Install via [nuget](http://www.nuget.org/):

    PM> Install-Package HtmlTags

Or for ASP.NET Core:

    PM> Install-Package HtmlTags.AspNetCore

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

What if you wanted to apply a CSS class to that input? Or change the element id?
You can do it, but it requires a much more verbose overload:

    <%: Html.TextBox("FirstName", "Lucas", new Dictionary<string, object> {{"id", "first-name"}, {"class", "required"}) %>

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

### Custom Data attributes

There is also built-in support for JSON serializing values of HTML5-style custom data attributes:

    <%: Html.TextBoxTag("Birthday")
        .Data("validate", true)
        .Data("validate-type", "date")
        .Data("min-date", new DateTime(1900, 1, 1)) %>

Will render:

    <input id="Birthday" name="Birthday" type="text" data-validate="true" data-validate-type="date" data-min-date="&quot;\/Date(-2208967200000)\/&quot;" />

### jQuery Metadata support

Or you can serialize an entire settings object in a single value on the server that can be interpreted on the client using something like the [jQuery Metadata plugin](http://docs.jquery.com/Plugins/Metadata):

    public class EditableOptions {
        public bool MultiLine { get; set; }
        public int MaxLength { get; set; }
        public string InputType { get; set; }
    }

    public static HtmlTag Editable(this HtmlHelper html) {
        // EditableOptions hardcoded for this example, but they could be 
        // determined based on different criteria for the input being rendered
        var options = new EditableOptions {
            MultiLine = false,
            MaxLength = 20,
            InputType = "date"
        };

        return new HtmlTag("div").MetaData("editable", options);
    }

Notice we're passing an instance of an object (EditableOptions) as the value of the "editable" metadata. This will render:

    <div data-__="{&quot;editable&quot;:{&quot;MultiLine&quot;:false,&quot;MaxLength&quot;:20,&quot;InputType&quot;:&quot;date&quot;}}"></div>

You can then drive logic on the client using this data:

    $.metadata.setType('attr', 'data-__'); //once on your page
    var o = $("div").metadata();
    alert(o.MultiLine); // # false
    alert(o.MaxLength); // # 20
    alert(o.InputType); // # "date"


### More information

More detailed documention is coming. In the meantime, try browsing the [unit tests](https://github.com/HtmlTags/htmltags/blob/master/test/HtmlTags.Testing/HtmlTagTester.cs) for further examples.
