// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Identity
{
    public enum IdentityFailure
    {
        DuplicateUserName,
        DuplicateEmail,
        DuplicateRoleName,
        UserValidationFailed,
        UserNameTooShort,
        RoleNameTooShort,
        UserNameInvalid,
        EmailInvalid,
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
        Unknown
    }
}