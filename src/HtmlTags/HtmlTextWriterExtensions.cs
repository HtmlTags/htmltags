using System.Text;
using System.IO;
#if DNXCORE50
using Microsoft.AspNet.Html;
using Microsoft.AspNet.Mvc.ViewFeatures;
#else
using System.Web.UI;
#endif

namespace HtmlTags
{
    public static class HtmlTextWriterExtensions
    {
#if DNXCORE50
        internal static HtmlTextWriter New(StringWriter ignored, string ignored2, string lineSeparator)
        {
            return new StringCollectionTextWriter(Encoding.UTF8) { NewLine = lineSeparator };
        }

        internal static void AddAttribute(this HtmlTextWriter writer, string key, string value, bool isEncoded)
        {
            //writer.
        }

        internal static void AddAttribute(this HtmlTextWriter writer, string key, string value)
        {
            //writer.
        }
#else
        internal static HtmlTextWriter New(StringWriter writer, string tabSeparator, string lineSeparator)
        {
            return new HtmlTextWriter(writer, tabSeparator) {NewLine = lineSeparator };
        }
#endif
    }
}

