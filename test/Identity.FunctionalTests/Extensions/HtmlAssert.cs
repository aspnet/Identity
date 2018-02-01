// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom.Html;
using Xunit;

namespace Microsoft.AspNetCore.Identity.FunctionalTests
{
    public class HtmlAssert
    {
        public static IHtmlFormElement HasForm(IHtmlDocument document)
        {
            var form = Assert.Single(document.QuerySelectorAll("form"));
            return Assert.IsAssignableFrom<IHtmlFormElement>(form);
        }

        public static IHtmlAnchorElement HasLink(string selector, IHtmlDocument document)
        {
            var element = Assert.Single(document.QuerySelectorAll(selector));
            return Assert.IsAssignableFrom<IHtmlAnchorElement>(element);
        }

        internal static IEnumerable<IHtmlElement> HasElements(string selector, IHtmlDocument document)
        {
            var elements = document
                .QuerySelectorAll(selector)
                .OfType<IHtmlElement>()
                .ToArray();

            Assert.True(elements.Length > 0);

            return elements;
        }

        public static IHtmlElement HasElement(string selector, IHtmlDocument document)
        {
            var element = Assert.Single(document.QuerySelectorAll(selector));
            return Assert.IsAssignableFrom<IHtmlElement>(element);
        }

        public static IHtmlFormElement HasForm(string selector, IHtmlDocument document)
        {
            var form = Assert.Single(document.QuerySelectorAll(selector));
            return Assert.IsAssignableFrom<IHtmlFormElement>(form);
        }
    }
}
