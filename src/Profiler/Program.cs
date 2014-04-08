using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HtmlTags;

namespace Profiler
{
    class Program
    {
        public static HtmlTag TestHtmlTag()
        {
            // Center the div tag
            var div = new HtmlTag("div")
                .AddClass("details-update-button-set");

            // Set up the update submit button
            var update = new HtmlTag("a")
                .Id("save-button")
                .Attr("tabindex", "0")
                .AddClass("snap-button k-button k-button-icontext")
                .Text("Update");

            // Set up the cancel button
            var cancel = new HtmlTag("a")
                .Id("cancel-button")
                .Attr("tabindex", "0")
                .AddClass("snap-button k-button k-button-icontext")
                .Text("Cancel");

            // Set the HTML for the div and return it
            return div.Append(update).Append(cancel);
        }

        /// <summary>
        /// Function to render the HTML block for the update/cancel buttons for a details edit form
        /// </summary>
        /// <param name="helper">Reference to the HTML helper class</param>
        /// <returns>HTML string for the buttons block</returns>
        public static HtmlString TestTagBuilder()
        {
            // Create the tags we need
            var div = new TagBuilder("div");
            var update = new TagBuilder("a");
            var cancel = new TagBuilder("a");

            // Center the div tag
            div.AddCssClass("details-update-button-set");

            // Set up the update submit button
            update.Attributes.Add("id", "save-button");
            update.Attributes.Add("tabindex", "0");
            update.AddCssClass("snap-button k-button k-button-icontext");
            update.SetInnerText("Update");

            // Set up the cancel button
            cancel.Attributes.Add("id", "cancel-button");
            cancel.Attributes.Add("tabindex", "0");
            cancel.AddCssClass("snap-button k-button k-button-icontext");
            cancel.SetInnerText("Cancel");

            // Set the HTML for the div and return it
            div.InnerHtml = update.ToString() + cancel.ToString();
            return new HtmlString(div.ToString());
        }

        static double OldCodeTest(
            long count)
        {
            var start = DateTime.Now;
            for (long i = 0; i < count; i++) {
                TestTagBuilder().ToString();
            }
            var elapsed = DateTime.Now - start;
            return elapsed.TotalMilliseconds;
        }

        static double NewCodeTest(
            long count)
        {
            var start = DateTime.Now;
            for (long i = 0; i < count; i++) {
                TestHtmlTag().ToString();
            }
            var elapsed = DateTime.Now - start;
            return elapsed.TotalMilliseconds;
        }

        static void Main(string[] args)
        {
            CssClassNameValidator.AllowInvalidCssClassNames = true;

            long count = 100000;
            Console.WriteLine("Timing old method that produces:");
            Console.WriteLine(TestTagBuilder().ToString());
            Console.WriteLine("Took: " + OldCodeTest(count) + " milliseconds\n\n");

            //Console.WriteLine("");
            Console.WriteLine("Timing new method that produces:");
            Console.WriteLine(TestHtmlTag().ToString());
            Console.WriteLine("Took: " + NewCodeTest(count) + " milliseconds\n\n");
        }
    }
}
