// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNet.Identity
{
    public class IdentityErrorDescriber
    {
        public static IdentityErrorDescriber Default = new IdentityErrorDescriber();

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

        public virtual string FormatUserNameTooShort(string name)
        {
            return Resources.FormatInvalidUserName(name);
        }

        public virtual string FormatInvalidUserName(string name)
        {
            return Resources.FormatInvalidUserName(name);
        }

        public virtual string FormatInvalidEmail(string email)
        {
            return Resources.FormatInvalidEmail(email);
        }

        public virtual string FormatDuplicateUserName(string name)
        {
            return Resources.FormatDuplicateUserName(name);
        }

        public virtual string FormatDuplicateEmail(string email)
        {
            return Resources.FormatDuplicateEmail(email);
        }

        public virtual string FormatInvalidRoleName(string name)
        {
            return Resources.FormatInvalidRoleName(name);
        }

        public virtual string FormatDuplicateRoleName(string name)
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

        public virtual string FormatUserAlreadyInRole(string role)
        {
            return Resources.FormatUserAlreadyInRole(role);
        }

        public virtual string FormatUserNotInRole(string role)
        {
            return Resources.FormatUserNotInRole(role);
        }

        public virtual string FormatPasswordTooShort(int length)
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