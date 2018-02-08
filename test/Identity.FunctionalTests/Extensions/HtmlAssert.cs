﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
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

            Assert.NotEmpty(elements);

            return elements;
        }

        public static IHtmlElement HasElement(string selector, IParentNode document)
        {
            var element = Assert.Single(document.QuerySelectorAll(selector));
            return Assert.IsAssignableFrom<IHtmlElement>(element);
        }

        public static IHtmlFormElement HasForm(string selector, IParentNode document)
        {
            var form = Assert.Single(document.QuerySelectorAll(selector));
            return Assert.IsAssignableFrom<IHtmlFormElement>(form);
        }

        internal static IHtmlHtmlElement IsHtmlFragment(string htmlMessage)
        {
            var synteticNode = $"<div>{htmlMessage}</div>";
            var fragment = Assert.Single(new HtmlParser().ParseFragment(htmlMessage, context: null));
            return Assert.IsAssignableFrom<IHtmlHtmlElement>(fragment);
        }
    }
}
