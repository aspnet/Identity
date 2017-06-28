using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Testing
{
    public static class HtmlAssert
    {
        public static TElement HasElement<TElement>(IParentNode context, string selector)
            where TElement : IHtmlElement
        {
            var element = context.QuerySelector(selector);
            if (!(element is TElement typedElement))
            {
                Assert.True(false, $"The element found with selector '{selector}' is not a '{typeof(TElement).Name}' element.");
            }
            else
            {
                return typedElement;
            }

            Assert.True(false, $"Can't find an element '{typeof(TElement).Name}' using selector '{selector}'");
            return default(TElement);
        }

        public static IHtmlFormElement HasForm(IDocument document, string name = null)
        {
            foreach (var form in document.Forms)
            {
                if (string.Equals(form.Name, name))
                {
                    return form;
                }
            }

            Assert.True(false, $"Can't find form using selector '{name}'");
            return null;
        }
    }
}
