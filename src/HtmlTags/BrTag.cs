namespace HtmlTags
{
#if ASPNETCORE
    using Microsoft.AspNetCore.Html;
#else
    using System.Web.UI;
#endif
    public class BrTag : HtmlTag
    {
        public BrTag() : base("br")
        {
        }

        public static ComplianceModes ComplianceMode { get; set; }

        public enum ComplianceModes
        {
            AspNet = 0,
            Xhtml = 1,
            Html5 = 2,
        }

        protected override void WriteHtml(HtmlTextWriter html)
        {
            switch(BrTag.ComplianceMode)
            {
                case ComplianceModes.Xhtml:
                    html.Write("<br />");
                    break;
                case ComplianceModes.Html5:
                    html.Write("<br>");
                    break;
                case ComplianceModes.AspNet:
                default:
                    html.Write("<br />");
                    break;
            }
        }
    }
}