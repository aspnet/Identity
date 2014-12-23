// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Represents the result of an sign in operation
    /// </summary>
    public class SignInResult
    {
        private static readonly SignInResult _success = new SignInResult(true);

        /// <summary>
        ///     Failure constructor that takes error messages
        /// </summary>
        /// <param name="errors"></param>
        public SignInResult(SignInFailure failure)
        {
            Failure = failure;
            Succeeded = false;
        }

        protected SignInResult(bool success)
        {
            Succeeded = success;
        }

        /// <summary>
        ///     True if the operation was successful
        /// </summary>
        public bool Succeeded { get; private set; }

        public SignInFailure Failure { get; private set; }

        /// <summary>
        ///     Static success result
        /// </summary>
        /// <returns></returns>
        public static SignInResult Success
        {
            get { return _success; }
        }
    }
}