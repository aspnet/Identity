// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Identity
{
    public class IdentityErrorDescriber
    {
        public static IdentityErrorDescriber Default = new IdentityErrorDescriber();

        public virtual string DuplicateUserName()
        {
            return Resources.DuplicateUserName;
        }

        public virtual string DuplicateEmail()
        {
            return Resources.DuplicateEmail;
        }

        public virtual string DuplicateRoleName()
        {
            return Resources.DuplicateRoleName;
        }

        public virtual string PasswordMismatch()
        {
            return Resources.PasswordMismatch;
        }

        public virtual string InvalidToken()
        {
            return Resources.InvalidToken;
        }

        public virtual string LoginAlreadyAssociated()
        {
            return Resources.LoginAlreadyAssociated;
        }

        public virtual string FormatUserNameTooShort(object name)
        {
            return Resources.FormatInvalidUserName(name);
        }

        public virtual string FormatInvalidUserName(object name)
        {
            return Resources.FormatInvalidUserName(name);
        }

        public virtual string FormatInvalidEmail(object email)
        {
            return Resources.FormatInvalidEmail(email);
        }

        public virtual string FormatDuplicateUserName(object name)
        {
            return Resources.FormatDuplicateUserName(name);
        }

        public virtual string FormatDuplicateEmail(object email)
        {
            return Resources.FormatDuplicateEmail(email);
        }

        public virtual string FormatInvalidRoleName(object name)
        {
            return Resources.FormatInvalidRoleName(name);
        }

        public virtual string FormatDuplicateRoleName(object name)
        {
            return Resources.FormatDuplicateRoleName(name);
        }

        public virtual string UserAlreadyHasPassword()
        {
            return Resources.UserAlreadyHasPassword;
        }

        public virtual string UserLockoutNotEnabled()
        {
            return Resources.UserLockoutNotEnabled;
        }

        public virtual string FormatUserAlreadyInRole(object role)
        {
            return Resources.FormatUserAlreadyInRole(role);
        }

        public virtual string FormatUserNotInRole(object role)
        {
            return Resources.FormatUserNotInRole(role);
        }

        public virtual string FormatPasswordTooShort(object length)
        {
            return Resources.FormatPasswordTooShort(length);
        }

        public virtual string PasswordRequiresNonLetterAndDigit()
        {
            return Resources.PasswordRequiresNonLetterAndDigit;
        }

        public virtual string PasswordRequiresDigit()
        {
            return Resources.PasswordRequiresDigit;
        }

        public virtual string PasswordRequiresLower()
        {
            return Resources.PasswordRequiresLower;
        }

        public virtual string PasswordRequiresUpper()
        {
            return Resources.PasswordRequiresUpper;
        }
    }
}