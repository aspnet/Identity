using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    class HtmlAssert
    {
        internal static IHtmlAnchorElement HasLinkWithText(IHtmlDocument document, string linkText)
        {
            return HasLinkWithText(document, linkText, StringComparison.Ordinal);
        }

        private static IHtmlAnchorElement HasLinkWithText(IHtmlDocument document, string linkText, StringComparison comparison)
        {
            IHtmlAnchorElement result = null;
            for (int i = 0; i < document.Links.Length; i++)
            {
                var link = document.Links[i];
                if (link is IHtmlAnchorElement anchor && link.InnerHtml.Equals(linkText, comparison))
                {
                    result = anchor;
                    break;
                }
            }

            Assert.NotNull(result);
            return result;
        }

        public static IHtmlFormElement HasForm(IHtmlDocument register)
        {
            Assert.Equal(1, register.Forms.Length);
            return register.Forms[0];
        }
    }
}
