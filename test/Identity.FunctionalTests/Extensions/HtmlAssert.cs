// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class HtmlAssert
    {
        public static IHtmlFormElement HasForm(IHtmlDocument element)
        {
            var form = Assert.Single(element.QuerySelectorAll("form"));
            return Assert.IsAssignableFrom<IHtmlFormElement>(form);
        }

        public static IHtmlAnchorElement HasLink(string selector, IHtmlDocument index)
        {
            var element = index.QuerySelectorAll(selector);
            Assert.Equal(1, element.Length);
            var link = Assert.IsAssignableFrom<IHtmlAnchorElement>(element[0]);

            return link;
        }
    }
}
