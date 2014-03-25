using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.HttpFeature.Security;

namespace Microsoft.AspNet.Identity
{
    public interface IClaimsIdentityProvider<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<ClaimsIdentity> CreateUserIdentity(UserManager<TUser, TKey> manager, TUser user);
    }

    public class AuthenticationSignIn : IAuthenticationSignIn
    {
        public ClaimsPrincipal User { get; set; }
        public IDictionary<string, string> Properties { get; set; }
    }

    public class ClaimsIdentityProvider<TUser, TKey> : IClaimsIdentityProvider<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {

        public async virtual Task<ClaimsIdentity> CreateUserIdentity(UserManager<TUser, TKey> manager, TUser user)
        {
            return await manager.CreateIdentity(user, "Microsoft.AspNet.Identity");
        }
    }

    public class SignInHelper<TUser, TKey>
        where TUser : class, IUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public SignInHelper(UserManager<TUser, TKey> userManager, IClaimsIdentityProvider<TUser, TKey> claimsIdentityProvider)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            if (claimsIdentityProvider == null)
            {
                throw new ArgumentNullException("claimsIdentityProvider");
            }
            UserManager = userManager;
            ClaimsIdentityProvider = claimsIdentityProvider;
        }

        public UserManager<TUser, TKey> UserManager { get; private set; }
        public IClaimsIdentityProvider<TUser, TKey> ClaimsIdentityProvider { get; set; }

        public virtual async Task<IAuthenticationSignIn> SignIn(TUser user, bool isPersistent, bool rememberBrowser)
        {
            // TODO: all the two factor logic/external
            // TODO: set isPersistent
            var authSignIn = new AuthenticationSignIn
            {
                User = new ClaimsPrincipal(), 
                Properties = new Dictionary<string, string>()
            };
            authSignIn.User.AddIdentity(await ClaimsIdentityProvider.CreateUserIdentity(UserManager, user));
            return authSignIn;
        }

        //public virtual async Task<bool> SendTwoFactorCode(string provider)
        //{
        //    var userId = await GetVerifiedUserId();
        //    if (userId == null)
        //    {
        //        return false;
        //    }

        //    var token = await UserManager.GenerateTwoFactorToken(userId, provider);
        //    // See IdentityConfig.cs to plug in Email/SMS services to actually send the code
        //    await UserManager.NotifyTwoFactorToken(userId, provider, token);
        //    return true;
        //}

        //public Task<TKey> GetVerifiedUserId()
        //{
        //    //var result = await AuthenticationManager.Authenticate(DefaultAuthenticationTypes.TwoFactorCookie);
        //    //if (result != null && result.Identity != null && !String.IsNullOrEmpty(result.Identity.GetUserId()))
        //    //{
        //    //    return result.Identity.GetUserId();
        //    //}
        //    return Task.FromResult(default(TKey));
        //}

        //public async Task<bool> HasBeenVerified()
        //{
        //    return await GetVerifiedUserId() != null;
        //}

        //public virtual async Task<SignInStatus> TwoFactorSignIn(string provider, string code, bool isPersistent, bool rememberBrowser)
        //{
        //    var userId = await GetVerifiedUserId();
        //    if (userId == null)
        //    {
        //        return SignInStatus.Failure;
        //    }
        //    var user = await UserManager.FindById(userId);
        //    if (user == null)
        //    {
        //        return SignInStatus.Failure;
        //    }
        //    if (await UserManager.IsLockedOut(user.Id))
        //    {
        //        return SignInStatus.LockedOut;
        //    }
        //    if (await UserManager.VerifyTwoFactorToken(user.Id, provider, code))
        //    {
        //        // When token is verified correctly, clear the access failed count used for lockout
        //        await UserManager.ResetAccessFailedCount(user.Id);
        //        await SignIn(user, isPersistent, rememberBrowser);
        //        return SignInStatus.Success;
        //    }
        //    // If the token is incorrect, record the failure which also may cause the user to be locked out
        //    await UserManager.AccessFailed(user.Id);
        //    return SignInStatus.Failure;
        //}

        //public async Task<SignInStatus> ExternalSignIn(ExternalLoginInfo loginInfo, bool isPersistent)
        //{
        //    var user = await UserManager.Find(loginInfo.Login);
        //    if (user == null)
        //    {
        //        return SignInStatus.Failure;
        //    }
        //    if (await UserManager.IsLockedOut(user.Id))
        //    {
        //        return SignInStatus.LockedOut;
        //    }
        //    return await SignInOrTwoFactor(user, isPersistent);
        //}

        //private async Task<SignInStatus> SignInOrTwoFactor(TUser user, bool isPersistent)
        //{
        //    if (await UserManager.GetTwoFactorEnabled(user.Id))
        //        //&& !await AuthenticationManager.TwoFactorBrowserRemembered(user.Id))
        //    {
        //        //var identity = new ClaimsIdentity(DefaultAuthenticationTypes.TwoFactorCookie);
        //        //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
        //        //AuthenticationManager.SignIn(identity);
        //        return SignInStatus.RequiresTwoFactorAuthentication;
        //    }
        //    await SignIn(user, isPersistent, false);
        //    return SignInStatus.Success;
        //}

        public virtual async Task<SignInStatus> PasswordSignIn(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await UserManager.FindByName(userName);
            if (user == null)
            {
                return SignInStatus.Failure;
            }
            if (await UserManager.IsLockedOut(user.Id))
            {
                return SignInStatus.LockedOut;
            }
            if (await UserManager.CheckPassword(user, password))
            {
                await SignIn(user, isPersistent, false);
                return SignInStatus.Success;
                //TODO: return await SignInOrTwoFactor(user, isPersistent);
            }
            if (shouldLockout)
            {
                // If lockout is requested, increment access failed count which might lock out the user
                await UserManager.AccessFailed(user.Id);
                if (await UserManager.IsLockedOut(user.Id))
                {
                    return SignInStatus.LockedOut;
                }
            }
            return SignInStatus.Failure;
        }
    }

    public enum SignInStatus
    {
        Success,
        LockedOut,
        RequiresTwoFactorAuthentication,
        Failure
    }

}