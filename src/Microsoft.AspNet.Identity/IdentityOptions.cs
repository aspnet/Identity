// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNet.Identity
{
    /// <summary>
    /// Represents all the options you can use to configure the identity system.
    /// </summary>
    public class IdentityOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="ClaimsIdentityOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="ClaimsIdentityOptions"/> for the identity system.
        /// </value>
        public ClaimsIdentityOptions ClaimsIdentity { get; set; } = new ClaimsIdentityOptions();

        /// <summary>
        /// Gets or sets the <see cref="UserOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="UserOptions"/> for the identity system.
        /// </value>
        public UserOptions User { get; set; } = new UserOptions();

        /// <summary>
        /// Gets or sets the <see cref="PasswordOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="PasswordOptions"/> for the identity system.
        /// </value>
        public PasswordOptions Password { get; set; } = new PasswordOptions();

        /// <summary>
        /// Gets or sets the <see cref="LockoutOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="LockoutOptions"/> for the identity system.
        /// </value>
        public LockoutOptions Lockout { get; set; } = new LockoutOptions();

        /// <summary>
        /// Gets or sets the <see cref="SignInOptions"/> for the identity system.
        /// </summary>
        /// <value>
        /// The <see cref="SignInOptions"/> for the identity system.
        /// </value>
        public SignInOptions SignIn { get; set; } = new SignInOptions();

        /// <summary>
        /// Gets or sets the <see cref="TimeSpan"/> after which security stamps are re-validated.
        /// </summary>
        /// <value>
        /// The <see cref="TimeSpan"/> after which security stamps are re-validated.
        /// </value>
        public TimeSpan SecurityStampValidationInterval { get; set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Gets or sets the <see cref="EmailConfirmationTokenProvider"/> used to generate tokens used in account confirmation emails.
        /// </summary>
        /// <value>
        /// The <see cref="EmailConfirmationTokenProvider"/> used to generate tokens used in account confirmation emails.
        /// </value>
        public string EmailConfirmationTokenProvider { get; set; } = Resources.DefaultTokenProvider;

        /// <summary>
        /// Gets or sets the <see cref="PasswordResetTokenProvider"/> used to generate tokens used in password reset emails.
        /// </summary>
        /// <value>
        /// The <see cref="PasswordResetTokenProvider"/> used to generate tokens used in password reset emails.
        /// </value>
        public string PasswordResetTokenProvider { get; set; } = Resources.DefaultTokenProvider;

        /// <summary>
        /// Gets or sets the <see cref="ChangeEmailTokenProvider"/> used to generate tokens used in email change confirmation emails.
        /// </summary>
        /// <value>
        /// The <see cref="ChangeEmailTokenProvider"/> used to generate tokens used in email change confirmation emails.
        /// </value>
        public string ChangeEmailTokenProvider { get; set; } = Resources.DefaultTokenProvider;

        /// <summary>
        /// Gets or sets the scheme used to identify application authentication cookies, must match the scheme in IdentityCookieOptions.
        /// </summary>
        /// <value>The scheme used to identify application authentication cookies.</value>
        public string ApplicationCookieAuthenticationScheme { get; set; } = IdentityCookieOptions.ApplicationCookieAuthenticationScheme;

        /// <summary>
        /// Gets or sets the scheme used to identify external authentication cookies, must match the scheme in IdentityCookieOptions.
        /// </summary>
        /// <value>The scheme used to identify external authentication cookies.</value>
        public string ExternalCookieAuthenticationScheme { get; set; } = IdentityCookieOptions.ExternalCookieAuthenticationScheme;

        /// <summary>
        /// Gets or sets the scheme used to identify Two Factor authentication cookies for round tripping user identities, must match the scheme in IdentityCookieOptions.
        /// </summary>
        /// <value>The scheme used to identify user identity 2fa authentication cookies.</value>
        public string TwoFactorUserIdCookieAuthenticationScheme { get; set; } = IdentityCookieOptions.TwoFactorUserIdCookieAuthenticationScheme;

        /// <summary>
        /// Gets or sets the scheme used to identify Two Factor authentication cookies for saving the Remember Me state,, must match the scheme in IdentityCookieOptions
        /// </summary>
        /// <value>The scheme used to identify remember me application authentication cookies.</value>        
        public string TwoFactorRememberMeCookieAuthenticationScheme { get; set; } = IdentityCookieOptions.TwoFactorRememberMeCookieAuthenticationScheme;
    }
}