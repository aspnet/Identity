// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Identity
{
    public class IdentityFailureDescriber
    {
        public virtual string Describe(IdentityFailure failure)
        {
            switch (failure)
            {
                case IdentityFailure.UserLockedOut:
                case IdentityFailure.DuplicateUserName:
                case IdentityFailure.DuplicateEmail:
                case IdentityFailure.DuplicateRoleName:
                case IdentityFailure.UserValidationFailed:
                case IdentityFailure.UserNameTooShort:
                case IdentityFailure.RoleNameTooShort:
                case IdentityFailure.UserNameHasInvalidCharacters:
                case IdentityFailure.UserAlreadyInRole:
                case IdentityFailure.UserAlreadyHasPassword:
                case IdentityFailure.LoginAlreadyAssociated:
                case IdentityFailure.UserNotInRole:
                case IdentityFailure.LockoutForUserNotEnabled:
                case IdentityFailure.RoleValidationFailed:
                case IdentityFailure.InvalidEmail:
                case IdentityFailure.InvalidToken:
                case IdentityFailure.PasswordMismatch:
                case IdentityFailure.PasswordRequiresDigit:
                case IdentityFailure.PasswordRequiresLower:
                case IdentityFailure.PasswordRequiresUpper:
                case IdentityFailure.PasswordTooShort:
                case IdentityFailure.PasswordRequiresNonLetterAndDigit:
                case IdentityFailure.Unknown:
                default:
                    return failure.ToString();
            }
        }
    }

    public enum IdentityFailure
    {
        DuplicateUserName,
        DuplicateEmail,
        DuplicateRoleName,
        UserValidationFailed,
        UserNameTooShort,
        UserNameNotFound,
        RoleNameTooShort,
        UserNameHasInvalidCharacters,
        UserAlreadyInRole,
        UserAlreadyHasPassword,
        LoginAlreadyAssociated,
        UserNotInRole,
        LockoutForUserNotEnabled,
        RoleValidationFailed,
        InvalidEmail,
        InvalidToken,
        PasswordMismatch,
        PasswordRequiresDigit,
        PasswordRequiresLower,
        PasswordRequiresUpper,
        PasswordTooShort,
        PasswordRequiresNonLetterAndDigit,
        UserLockedOut,
        LoginNotFound,
        SignInRequiresTwoFactor,
        TwoFactorVerificationFailed,
        Unknown
    }
}