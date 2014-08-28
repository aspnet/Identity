// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Http.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Http
{
    /// <summary>
    ///     Extensions methods on IAuthenticationManager that add methods for using the default Application and External
    ///     authentication type constants
    /// </summary>
    public static class IdentityHttpContextExtensions
    {
        /// <summary>
        ///     Return the authentication types which are considered external because they have captions
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static IEnumerable<AuthenticationDescription> GetExternalAuthenticationTypes(
            this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("manager");
            }
            return context.GetAuthenticationTypes().Where(d => !String.IsNullOrEmpty(d.Caption));
        }
    }
}