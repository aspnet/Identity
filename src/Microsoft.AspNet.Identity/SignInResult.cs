// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    ///     Represents the result of an sign in operation
    /// </summary>
    public class SignInResult
    {
        /// <summary>
        ///     True if the operation was successful
        /// </summary>
        public bool Succeeded { get; protected set; }

        /// <summary>
        ///     True if the user is locked out
        /// </summary>
        public bool IsLockedOut { get; protected set; }

        /// <summary>
        ///     True if the user is not allowed to sign in
        /// </summary>
        public bool IsNotAllowed { get; protected set; }

        /// <summary>
        ///     True if the sign in requires two factor
        /// </summary>
        public bool RequiresTwoFactor { get; protected set; }

        /// <summary>
        ///     Static success result
        /// </summary>
        /// <returns></returns>
        public static SignInResult Success => new SignInResult { Succeeded = true };

        /// <summary>
        ///     Static failure result
        /// </summary>
        /// <returns></returns>
        public static SignInResult Failed => new SignInResult();

        /// <summary>
        ///     Static locked out result
        /// </summary>
        /// <returns></returns>
        public static SignInResult LockedOut => new SignInResult { IsLockedOut = true };

        /// <summary>
        ///     Static not allowed result
        /// </summary>
        /// <returns></returns>
        public static SignInResult NotAllowed => new SignInResult { IsNotAllowed = true };

        /// <summary>
        ///     Static two factor required result
        /// </summary>
        /// <returns></returns>
        public static SignInResult TwoFactorRequired => new SignInResult { RequiresTwoFactor = true };

        /// <summary>
        ///     Returns string implemenation of the result. Provides string value for the assigned property
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var status = "";

            if (IsLockedOut)
            {
                status = "Lockedout";
            }
            else if (IsNotAllowed)
            {
                status = "NotAllowed";
            }
            else if (RequiresTwoFactor)
            {
                status = "RequiresTwoFactor";
            }
            else if (Succeeded)
            {
                status = "Succeeded";
            }
            else
            {
                status = "Failed";
            }

            return status;
        }
    }
}