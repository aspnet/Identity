using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Identity
{
	/// <summary>
	/// Provides the APIs for managing user in a persistence store.
	/// </summary>
	/// <typeparam name="TUser">The type encapsulating a user.</typeparam>
	public interface IUserManager<TUser> : IDisposable where TUser : class
	{
		/// <summary>
		/// The <see cref="ILogger"/> used to log messages from the manager.
		/// </summary>
		/// <value>
		/// The <see cref="ILogger"/> used to log messages from the manager.
		/// </value>
		ILogger Logger { get; set; }

		/// <summary>
		/// The <see cref="IPasswordHasher{TUser}"/> used to hash passwords.
		/// </summary>
		IPasswordHasher<TUser> PasswordHasher { get; set; }

		/// <summary>
		/// The <see cref="IUserValidator{TUser}"/> used to validate users.
		/// </summary>
		IList<IUserValidator<TUser>> UserValidators { get; }

		/// <summary>
		/// The <see cref="IPasswordValidator{TUser}"/> used to validate passwords.
		/// </summary>
		IList<IPasswordValidator<TUser>> PasswordValidators { get; }

		/// <summary>
		/// The <see cref="ILookupNormalizer"/> used to normalize things like user and role names.
		/// </summary>
		ILookupNormalizer KeyNormalizer { get; set; }

		/// <summary>
		/// The <see cref="IdentityErrorDescriber"/> used to generate error messages.
		/// </summary>
		IdentityErrorDescriber ErrorDescriber { get; set; }

		/// <summary>
		/// The <see cref="IdentityOptions"/> used to configure Identity.
		/// </summary>
		IdentityOptions Options { get; set; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports authentication tokens.
		/// </summary>
		/// <value>
		/// true if the backing user store supports authentication tokens, otherwise false.
		/// </value>
		bool SupportsUserAuthenticationTokens { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports a user authenticator.
		/// </summary>
		/// <value>
		/// true if the backing user store supports a user authenticator, otherwise false.
		/// </value>
		bool SupportsUserAuthenticatorKey { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports recovery codes.
		/// </summary>
		/// <value>
		/// true if the backing user store supports a user authenticator, otherwise false.
		/// </value>
		bool SupportsUserTwoFactorRecoveryCodes { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports two factor authentication.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user two factor authentication, otherwise false.
		/// </value>
		bool SupportsUserTwoFactor { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports user passwords.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user passwords, otherwise false.
		/// </value>
		bool SupportsUserPassword { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports security stamps.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user security stamps, otherwise false.
		/// </value>
		bool SupportsUserSecurityStamp { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports user roles.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user roles, otherwise false.
		/// </value>
		bool SupportsUserRole { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports external logins.
		/// </summary>
		/// <value>
		/// true if the backing user store supports external logins, otherwise false.
		/// </value>
		bool SupportsUserLogin { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports user emails.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user emails, otherwise false.
		/// </value>
		bool SupportsUserEmail { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports user telephone numbers.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user telephone numbers, otherwise false.
		/// </value>
		bool SupportsUserPhoneNumber { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports user claims.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user claims, otherwise false.
		/// </value>
		bool SupportsUserClaim { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports user lock-outs.
		/// </summary>
		/// <value>
		/// true if the backing user store supports user lock-outs, otherwise false.
		/// </value>
		bool SupportsUserLockout { get; }

		/// <summary>
		/// Gets a flag indicating whether the backing user store supports returning
		/// <see cref="IQueryable"/> collections of information.
		/// </summary>
		/// <value>
		/// true if the backing user store supports returning <see cref="IQueryable"/> collections of
		/// information, otherwise false.
		/// </value>
		bool SupportsQueryableUsers { get; }

		/// <summary>
		///     Returns an IQueryable of users if the store is an IQueryableUserStore
		/// </summary>
		IQueryable<TUser> Users { get; }

		/// <summary>
		/// Returns the Name claim value if present otherwise returns null.
		/// </summary>
		/// <param name="principal">The <see cref="ClaimsPrincipal"/> instance.</param>
		/// <returns>The Name claim value, or null if the claim is not present.</returns>
		/// <remarks>The Name claim is identified by <see cref="ClaimsIdentity.DefaultNameClaimType"/>.</remarks>
		string GetUserName(ClaimsPrincipal principal);

		/// <summary>
		/// Returns the User ID claim value if present otherwise returns null.
		/// </summary>
		/// <param name="principal">The <see cref="ClaimsPrincipal"/> instance.</param>
		/// <returns>The User ID claim value, or null if the claim is not present.</returns>
		/// <remarks>The User ID claim is identified by <see cref="ClaimTypes.NameIdentifier"/>.</remarks>
		string GetUserId(ClaimsPrincipal principal);

		/// <summary>
		/// Returns the user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
		/// the principal or null.
		/// </summary>
		/// <param name="principal">The principal which contains the user id claim.</param>
		/// <returns>The user corresponding to the IdentityOptions.ClaimsIdentity.UserIdClaimType claim in
		/// the principal or null</returns>
		Task<TUser> GetUserAsync(ClaimsPrincipal principal);

		/// <summary>
		/// Generates a value suitable for use in concurrency tracking.
		/// </summary>
		/// <param name="user">The user to generate the stamp for.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the security
		/// stamp for the specified <paramref name="user"/>.
		/// </returns>
		Task<string> GenerateConcurrencyStampAsync(TUser user);

		/// <summary>
		/// Creates the specified <paramref name="user"/> in the backing store with no password,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to create.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> CreateAsync(TUser user);

		/// <summary>
		/// Updates the specified <paramref name="user"/> in the backing store.
		/// </summary>
		/// <param name="user">The user to update.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> UpdateAsync(TUser user);

		/// <summary>
		/// Deletes the specified <paramref name="user"/> from the backing store.
		/// </summary>
		/// <param name="user">The user to delete.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> DeleteAsync(TUser user);

		/// <summary>
		/// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
		/// </summary>
		/// <param name="userId">The user ID to search for.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
		/// </returns>
		Task<TUser> FindByIdAsync(string userId);

		/// <summary>
		/// Finds and returns a user, if any, who has the specified user name.
		/// </summary>
		/// <param name="userName">The user name to search for.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userName"/> if it exists.
		/// </returns>
		Task<TUser> FindByNameAsync(string userName);

		/// <summary>
		/// Creates the specified <paramref name="user"/> in the backing store with given password,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user to create.</param>
		/// <param name="password">The password for the user to hash and store.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> CreateAsync(TUser user, string password);

		/// <summary>
		/// Normalize a key (user name, email) for consistent comparisons.
		/// </summary>
		/// <param name="key">The key to normalize.</param>
		/// <returns>A normalized value representing the specified <paramref name="key"/>.</returns>
		string NormalizeKey(string key);

		/// <summary>
		/// Updates the normalized user name for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose user name should be normalized and updated.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task UpdateNormalizedUserNameAsync(TUser user);

		/// <summary>
		/// Gets the user name for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose name should be retrieved.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
		Task<string> GetUserNameAsync(TUser user);

		/// <summary>
		/// Sets the given <paramref name="userName" /> for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose name should be set.</param>
		/// <param name="userName">The user name to set.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task<IdentityResult> SetUserNameAsync(TUser user, string userName);

		/// <summary>
		/// Gets the user identifier for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose identifier should be retrieved.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
		Task<string> GetUserIdAsync(TUser user);

		/// <summary>
		/// Returns a flag indicating whether the given <paramref name="password"/> is valid for the
		/// specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose password should be validated.</param>
		/// <param name="password">The password to validate</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if
		/// the specified <paramref name="password" /> matches the one store for the <paramref name="user"/>,
		/// otherwise false.</returns>
		Task<bool> CheckPasswordAsync(TUser user, string password);

		/// <summary>
		/// Gets a flag indicating whether the specified <paramref name="user"/> has a password.
		/// </summary>
		/// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a password
		/// otherwise false.
		/// </returns>
		Task<bool> HasPasswordAsync(TUser user);

		/// <summary>
		/// Adds the <paramref name="password"/> to the specified <paramref name="user"/> only if the user
		/// does not already have a password.
		/// </summary>
		/// <param name="user">The user whose password should be set.</param>
		/// <param name="password">The password to set.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> AddPasswordAsync(TUser user, string password);

		/// <summary>
		/// Changes a user's password after confirming the specified <paramref name="currentPassword"/> is correct,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose password should be set.</param>
		/// <param name="currentPassword">The current password to validate before changing.</param>
		/// <param name="newPassword">The new password to set for the specified <paramref name="user"/>.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

		/// <summary>
		/// Removes a user's password.
		/// </summary>
		/// <param name="user">The user whose password should be removed.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> RemovePasswordAsync(TUser user);

		/// <summary>
		/// Get the security stamp for the specified <paramref name="user" />.
		/// </summary>
		/// <param name="user">The user whose security stamp should be set.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
		Task<string> GetSecurityStampAsync(TUser user);

		/// <summary>
		/// Regenerates the security stamp for the specified <paramref name="user" />.
		/// </summary>
		/// <param name="user">The user whose security stamp should be regenerated.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		/// <remarks>
		/// Regenerating a security stamp will sign out any saved login for the user.
		/// </remarks>
		Task<IdentityResult> UpdateSecurityStampAsync(TUser user);

		/// <summary>
		/// Generates a password reset token for the specified <paramref name="user"/>, using
		/// the configured password reset token provider.
		/// </summary>
		/// <param name="user">The user to generate a password reset token for.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation,
		/// containing a password reset token for the specified <paramref name="user"/>.</returns>
		Task<string> GeneratePasswordResetTokenAsync(TUser user);

		/// <summary>
		/// Resets the <paramref name="user"/>'s password to the specified <paramref name="newPassword"/> after
		/// validating the given password reset <paramref name="token"/>.
		/// </summary>
		/// <param name="user">The user whose password should be reset.</param>
		/// <param name="token">The password reset token to verify.</param>
		/// <param name="newPassword">The new password to set if reset token verification succeeds.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> ResetPasswordAsync(TUser user, string token, string newPassword);

		/// <summary>
		/// Retrieves the user associated with the specified external login provider and login provider key.
		/// </summary>
		/// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
		/// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
		/// <returns>
		/// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
		/// </returns>
		Task<TUser> FindByLoginAsync(string loginProvider, string providerKey);

		/// <summary>
		/// Attempts to remove the provided external login information from the specified <paramref name="user"/>.
		/// and returns a flag indicating whether the removal succeed or not.
		/// </summary>
		/// <param name="user">The user to remove the login information from.</param>
		/// <param name="loginProvider">The login provide whose information should be removed.</param>
		/// <param name="providerKey">The key given by the external login provider for the specified user.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> RemoveLoginAsync(TUser user, string loginProvider, string providerKey);

		/// <summary>
		/// Adds an external <see cref="UserLoginInfo"/> to the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to add the login to.</param>
		/// <param name="login">The external <see cref="UserLoginInfo"/> to add to the specified <paramref name="user"/>.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> AddLoginAsync(TUser user, UserLoginInfo login);

		/// <summary>
		/// Retrieves the associated logins for the specified <param ref="user"/>.
		/// </summary>
		/// <param name="user">The user whose associated logins to retrieve.</param>
		/// <returns>
		/// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
		/// </returns>
		Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user);

		/// <summary>
		/// Adds the specified <paramref name="claim"/> to the <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to add the claim to.</param>
		/// <param name="claim">The claim to add.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> AddClaimAsync(TUser user, Claim claim);

		/// <summary>
		/// Adds the specified <paramref name="claims"/> to the <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to add the claim to.</param>
		/// <param name="claims">The claims to add.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> AddClaimsAsync(TUser user, IEnumerable<Claim> claims);

		/// <summary>
		/// Replaces the given <paramref name="claim"/> on the specified <paramref name="user"/> with the <paramref name="newClaim"/>
		/// </summary>
		/// <param name="user">The user to replace the claim on.</param>
		/// <param name="claim">The claim to replace.</param>
		/// <param name="newClaim">The new claim to replace the existing <paramref name="claim"/> with.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim);

		/// <summary>
		/// Removes the specified <paramref name="claim"/> from the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to remove the specified <paramref name="claim"/> from.</param>
		/// <param name="claim">The <see cref="Claim"/> to remove.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> RemoveClaimAsync(TUser user, Claim claim);

		/// <summary>
		/// Removes the specified <paramref name="claims"/> from the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to remove the specified <paramref name="claims"/> from.</param>
		/// <param name="claims">A collection of <see cref="Claim"/>s to remove.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims);

		/// <summary>
		/// Gets a list of <see cref="Claim"/>s to be belonging to the specified <paramref name="user"/> as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose claims to retrieve.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <see cref="Claim"/>s.
		/// </returns>
		Task<IList<Claim>> GetClaimsAsync(TUser user);

		/// <summary>
		/// Add the specified <paramref name="user"/> to the named role.
		/// </summary>
		/// <param name="user">The user to add to the named role.</param>
		/// <param name="role">The name of the role to add the user to.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> AddToRoleAsync(TUser user, string role);

		/// <summary>
		/// Add the specified <paramref name="user"/> to the named roles.
		/// </summary>
		/// <param name="user">The user to add to the named roles.</param>
		/// <param name="roles">The name of the roles to add the user to.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> AddToRolesAsync(TUser user, IEnumerable<string> roles);

		/// <summary>
		/// Removes the specified <paramref name="user"/> from the named role.
		/// </summary>
		/// <param name="user">The user to remove from the named role.</param>
		/// <param name="role">The name of the role to remove the user from.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> RemoveFromRoleAsync(TUser user, string role);

		/// <summary>
		/// Removes the specified <paramref name="user"/> from the named roles.
		/// </summary>
		/// <param name="user">The user to remove from the named roles.</param>
		/// <param name="roles">The name of the roles to remove the user from.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> RemoveFromRolesAsync(TUser user, IEnumerable<string> roles);

		/// <summary>
		/// Gets a list of role names the specified <paramref name="user"/> belongs to.
		/// </summary>
		/// <param name="user">The user whose role names to retrieve.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing a list of role names.</returns>
		Task<IList<string>> GetRolesAsync(TUser user);

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="user"/> is a member of the give named role.
		/// </summary>
		/// <param name="user">The user whose role membership should be checked.</param>
		/// <param name="role">The name of the role to be checked.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified <paramref name="user"/> is
		/// a member of the named role.
		/// </returns>
		Task<bool> IsInRoleAsync(TUser user, string role);

		/// <summary>
		/// Gets the email address for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose email should be returned.</param>
		/// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
		Task<string> GetEmailAsync(TUser user);

		/// <summary>
		/// Sets the <paramref name="email"/> address for a <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose email should be set.</param>
		/// <param name="email">The email to set.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> SetEmailAsync(TUser user, string email);

		/// <summary>
		/// Gets the user, if any, associated with the normalized value of the specified email address.
		/// </summary>
		/// <param name="email">The email address to return the user for.</param>
		/// <returns>
		/// The task object containing the results of the asynchronous lookup operation, the user, if any, associated with a normalized value of the specified email address.
		/// </returns>
		Task<TUser> FindByEmailAsync(string email);

		/// <summary>
		/// Updates the normalized email for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose email address should be normalized and updated.</param>
		/// <returns>The task object representing the asynchronous operation.</returns>
		Task UpdateNormalizedEmailAsync(TUser user);

		/// <summary>
		/// Generates an email confirmation token for the specified user.
		/// </summary>
		/// <param name="user">The user to generate an email confirmation token for.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, an email confirmation token.
		/// </returns>
		Task<string> GenerateEmailConfirmationTokenAsync(TUser user);

		/// <summary>
		/// Validates that an email confirmation token matches the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user to validate the token against.</param>
		/// <param name="token">The email confirmation token to validate.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);

		/// <summary>
		/// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
		/// false.
		/// </summary>
		/// <param name="user">The user whose email confirmation status should be returned.</param>
		/// <returns>
		/// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
		/// has been confirmed or not.
		/// </returns>
		Task<bool> IsEmailConfirmedAsync(TUser user);

		/// <summary>
		/// Generates an email change token for the specified user.
		/// </summary>
		/// <param name="user">The user to generate an email change token for.</param>
		/// <param name="newEmail">The new email address.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, an email change token.
		/// </returns>
		Task<string> GenerateChangeEmailTokenAsync(TUser user, string newEmail);

		/// <summary>
		/// Updates a users emails if the specified email change <paramref name="token"/> is valid for the user.
		/// </summary>
		/// <param name="user">The user whose email should be updated.</param>
		/// <param name="newEmail">The new email address.</param>
		/// <param name="token">The change email token to be verified.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> ChangeEmailAsync(TUser user, string newEmail, string token);

		/// <summary>
		/// Gets the telephone number, if any, for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose telephone number should be retrieved.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
		Task<string> GetPhoneNumberAsync(TUser user);

		/// <summary>
		/// Sets the phone number for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose phone number to set.</param>
		/// <param name="phoneNumber">The phone number to set.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> SetPhoneNumberAsync(TUser user, string phoneNumber);

		/// <summary>
		/// Sets the phone number for the specified <paramref name="user"/> if the specified
		/// change <paramref name="token"/> is valid.
		/// </summary>
		/// <param name="user">The user whose phone number to set.</param>
		/// <param name="phoneNumber">The phone number to set.</param>
		/// <param name="token">The phone number confirmation token to validate.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
		/// of the operation.
		/// </returns>
		Task<IdentityResult> ChangePhoneNumberAsync(TUser user, string phoneNumber, string token);

		/// <summary>
		/// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed.
		/// </summary>
		/// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
		/// telephone number otherwise false.
		/// </returns>
		Task<bool> IsPhoneNumberConfirmedAsync(TUser user);

		/// <summary>
		/// Generates a telephone number change token for the specified user.
		/// </summary>
		/// <param name="user">The user to generate a telephone number token for.</param>
		/// <param name="phoneNumber">The new phone number the validation token should be sent to.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, containing the telephone change number token.
		/// </returns>
		Task<string> GenerateChangePhoneNumberTokenAsync(TUser user, string phoneNumber);

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="user"/>'s phone number change verification
		/// token is valid for the given <paramref name="phoneNumber"/>.
		/// </summary>
		/// <param name="user">The user to validate the token against.</param>
		/// <param name="token">The telephone number change token to validate.</param>
		/// <param name="phoneNumber">The telephone number the token was generated for.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, returning true if the <paramref name="token"/>
		/// is valid, otherwise false.
		/// </returns>
		Task<bool> VerifyChangePhoneNumberTokenAsync(TUser user, string token, string phoneNumber);

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="token"/> is valid for
		/// the given <paramref name="user"/> and <paramref name="purpose"/>.
		/// </summary>
		/// <param name="user">The user to validate the token against.</param>
		/// <param name="tokenProvider">The token provider used to generate the token.</param>
		/// <param name="purpose">The purpose the token should be generated for.</param>
		/// <param name="token">The token to validate</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, returning true if the <paramref name="token"/>
		/// is valid, otherwise false.
		/// </returns>
		Task<bool> VerifyUserTokenAsync(TUser user, string tokenProvider, string purpose, string token);

		/// <summary>
		/// Generates a token for the given <paramref name="user"/> and <paramref name="purpose"/>.
		/// </summary>
		/// <param name="purpose">The purpose the token will be for.</param>
		/// <param name="user">The user the token will be for.</param>
		/// <param name="tokenProvider">The provider which will generate the token.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents result of the asynchronous operation, a token for
		/// the given user and purpose.
		/// </returns>
		Task<string> GenerateUserTokenAsync(TUser user, string tokenProvider, string purpose);

		/// <summary>
		/// Registers a token provider.
		/// </summary>
		/// <param name="providerName">The name of the provider to register.</param>
		/// <param name="provider">The provider to register.</param>
		void RegisterTokenProvider(string providerName, IUserTwoFactorTokenProvider<TUser> provider);

		/// <summary>
		/// Gets a list of valid two factor token providers for the specified <paramref name="user"/>,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user the whose two factor authentication providers will be returned.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents result of the asynchronous operation, a list of two
		/// factor authentication providers for the specified user.
		/// </returns>
		Task<IList<string>> GetValidTwoFactorProvidersAsync(TUser user);

		/// <summary>
		/// Verifies the specified two factor authentication <paramref name="token" /> against the <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user the token is supposed to be for.</param>
		/// <param name="tokenProvider">The provider which will verify the token.</param>
		/// <param name="token">The token to verify.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents result of the asynchronous operation, true if the token is valid,
		/// otherwise false.
		/// </returns>
		Task<bool> VerifyTwoFactorTokenAsync(TUser user, string tokenProvider, string token);

		/// <summary>
		/// Gets a two factor authentication token for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user the token is for.</param>
		/// <param name="tokenProvider">The provider which will generate the token.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents result of the asynchronous operation, a two factor authentication token
		/// for the user.
		/// </returns>
		Task<string> GenerateTwoFactorTokenAsync(TUser user, string tokenProvider);

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose two factor authentication enabled status should be retrieved.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
		/// has two factor authentication enabled, otherwise false.
		/// </returns>
		Task<bool> GetTwoFactorEnabledAsync(TUser user);

		/// <summary>
		/// Sets a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose two factor authentication enabled status should be set.</param>
		/// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="IdentityResult"/> of the operation
		/// </returns>
		Task<IdentityResult> SetTwoFactorEnabledAsync(TUser user, bool enabled);

		/// <summary>
		/// Returns a flag indicating whether the specified <paramref name="user"/> his locked out,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose locked out status should be retrieved.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
		/// is locked out, otherwise false.
		/// </returns>
		Task<bool> IsLockedOutAsync(TUser user);

		/// <summary>
		/// Sets a flag indicating whether the specified <paramref name="user"/> is locked out,
		/// as an asynchronous operation.
		/// </summary>
		/// <param name="user">The user whose locked out status should be set.</param>
		/// <param name="enabled">Flag indicating whether the user is locked out or not.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="IdentityResult"/> of the operation
		/// </returns>
		Task<IdentityResult> SetLockoutEnabledAsync(TUser user, bool enabled);

		/// <summary>
		/// Retrieves a flag indicating whether user lockout can enabled for the specified user.
		/// </summary>
		/// <param name="user">The user whose ability to be locked out should be returned.</param>
		/// <returns>
		/// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
		/// </returns>
		Task<bool> GetLockoutEnabledAsync(TUser user);

		/// <summary>
		/// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any.
		/// Any time in the past should be indicates a user is not locked out.
		/// </summary>
		/// <param name="user">The user whose lockout date should be retrieved.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the lookup, a <see cref="DateTimeOffset"/> containing the last time a user's lockout expired, if any.
		/// </returns>
		Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user);

		/// <summary>
		/// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
		/// </summary>
		/// <param name="user">The user whose lockout date should be set.</param>
		/// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
		Task<IdentityResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd);

		/// <summary>
		/// Increments the access failed count for the user as an asynchronous operation.
		/// If the failed access account is greater than or equal to the configured maximum number of attempts,
		/// the user will be locked out for the configured lockout time span.
		/// </summary>
		/// <param name="user">The user whose failed access count to increment.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
		Task<IdentityResult> AccessFailedAsync(TUser user);

		/// <summary>
		/// Resets the access failed count for the specified <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose failed access count should be reset.</param>
		/// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
		Task<IdentityResult> ResetAccessFailedCountAsync(TUser user);

		/// <summary>
		/// Retrieves the current number of failed accesses for the given <paramref name="user"/>.
		/// </summary>
		/// <param name="user">The user whose access failed count should be retrieved for.</param>
		/// <returns>The <see cref="Task"/> that contains the result the asynchronous operation, the current failed access count
		/// for the user.</returns>
		Task<int> GetAccessFailedCountAsync(TUser user);

		/// <summary>
		/// Returns a list of users from the user store who have the specified <paramref name="claim"/>.
		/// </summary>
		/// <param name="claim">The claim to look for.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <typeparamref name="TUser"/>s who
		/// have the specified claim.
		/// </returns>
		Task<IList<TUser>> GetUsersForClaimAsync(Claim claim);

		/// <summary>
		/// Returns a list of users from the user store who are members of the specified <paramref name="roleName"/>.
		/// </summary>
		/// <param name="roleName">The name of the role whose users should be returned.</param>
		/// <returns>
		/// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a list of <typeparamref name="TUser"/>s who
		/// are members of the specified role.
		/// </returns>
		Task<IList<TUser>> GetUsersInRoleAsync(string roleName);

		/// <summary>
		/// Returns an authentication token for a user.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="loginProvider">The authentication scheme for the provider the token is associated with.</param>
		/// <param name="tokenName">The name of the token.</param>
		/// <returns>The authentication token for a user</returns>
		Task<string> GetAuthenticationTokenAsync(TUser user, string loginProvider, string tokenName);

		/// <summary>
		/// Sets an authentication token for a user.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="loginProvider">The authentication scheme for the provider the token is associated with.</param>
		/// <param name="tokenName">The name of the token.</param>
		/// <param name="tokenValue">The value of the token.</param>
		/// <returns>Whether the user was successfully updated.</returns>
		Task<IdentityResult> SetAuthenticationTokenAsync(TUser user, string loginProvider, string tokenName, string tokenValue);

		/// <summary>
		/// Remove an authentication token for a user.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="loginProvider">The authentication scheme for the provider the token is associated with.</param>
		/// <param name="tokenName">The name of the token.</param>
		/// <returns>Whether a token was removed.</returns>
		Task<IdentityResult> RemoveAuthenticationTokenAsync(TUser user, string loginProvider, string tokenName);

		/// <summary>
		/// Returns the authenticator key for the user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>The authenticator key</returns>
		Task<string> GetAuthenticatorKeyAsync(TUser user);

		/// <summary>
		/// Resets the authenticator key for the user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>Whether the user was successfully updated.</returns>
		Task<IdentityResult> ResetAuthenticatorKeyAsync(TUser user);

		/// <summary>
		/// Generates a new base32 encoded 160-bit security secret (size of SHA1 hash).
		/// </summary>
		/// <returns>The new security secret.</returns>
		string GenerateNewAuthenticatorKey();

		/// <summary>
		/// Generates recovery codes for the user, this invalidates any previous recovery codes for the user.
		/// </summary>
		/// <param name="user">The user to generate recovery codes for.</param>
		/// <param name="number">The number of codes to generate.</param>
		/// <returns>The new recovery codes for the user.  Note: there may be less than number returned, as duplicates will be removed.</returns>
		Task<IEnumerable<string>> GenerateNewTwoFactorRecoveryCodesAsync(TUser user, int number);

		/// <summary>
		/// Returns whether a recovery code is valid for a user. Note: recovery codes are only valid
		/// once, and will be invalid after use.
		/// </summary>
		/// <param name="user">The user who owns the recovery code.</param>
		/// <param name="code">The recovery code to use.</param>
		/// <returns>True if the recovery code was found for the user.</returns>
		Task<IdentityResult> RedeemTwoFactorRecoveryCodeAsync(TUser user, string code);

		/// <summary>
		/// Returns how many recovery code are still valid for a user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>How many recovery code are still valid for a user.</returns>
		Task<int> CountRecoveryCodesAsync(TUser user);

		/// <summary>
		/// Creates bytes to use as a security token from the user's security stamp.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>The security token bytes.</returns>
		Task<byte[]> CreateSecurityTokenAsync(TUser user);
	}
}