// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Represents the result of an identity operation
    /// </summary>
    public class IdentityResult
    {
        private static readonly IdentityResult _success = new IdentityResult(true);

        /// <summary>
        ///     Failure constructor that takes failures
        /// </summary>
        /// <param name="failures"></param>
        public IdentityResult(params IdentityFailure[] failures) : this((IEnumerable<IdentityFailure>)failures)
        {
        }

        /// <summary>
        ///     Failure constructor that takes error messages
        /// </summary>
        /// <param name="errors"></param>
        public IdentityResult(IEnumerable<IdentityFailure> failures)
        {
            if (failures == null || !failures.Any())
            {
                failures = new[] { IdentityFailure.Unknown };
            }
            Succeeded = false;
            Failures = failures;
        }

        protected IdentityResult(bool success)
        {
            Succeeded = success;
            Failures = new IdentityFailure[0];
        }

        /// <summary>
        ///     True if the operation was successful
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        ///     List of errors
        /// </summary>
        public IEnumerable<IdentityFailure> Failures { get; private set; }

        /// <summary>
        ///     Static success result
        /// </summary>
        /// <returns></returns>
        public static IdentityResult Success
        {
            get { return _success; }
        }

        /// <summary>
        ///     Failed helper method
        /// </summary>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static IdentityResult Failed(params IdentityFailure[] failures)
        {
            return new IdentityResult(failures);
        }
    }
}