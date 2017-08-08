using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Session;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Applications.Authentication
{
    /// <summary>
    /// Factory for Users and Applications.
    /// </summary>
    /// <typeparam name="TUser">The <typeparamref name="TUser"/>.</typeparam>
    /// <typeparam name="TApplication">The <typeparamref name="TApplication"/>.</typeparam>
    public class LoginContextFactory<TUser, TApplication> : ILoginFactory
        where TUser : class
        where TApplication : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<TUser> _userClaimsPrincipalFactory;
        private readonly ApplicationManager<TApplication> _applicationManager;
        private readonly IApplicationClaimsPrincipalFactory<TApplication> _applicationClaimsPrincipalFactory;

        /// <summary>
        ///  Creates a new instance of <see cref="LoginContextFactory{TUser, TApplication}"/>.
        /// </summary>
        /// <param name="userManager">The <see cref="UserManager{TUser}"/>.</param>
        /// <param name="userClaimsPrincipalFactory">The <see cref="IUserClaimsPrincipalFactory{TUser}"/>.</param>
        /// <param name="applicationManager">The <see cref="ApplicationManager{TApplication}"/>.</param>
        /// <param name="applicationClaimsPrincipalFactory">The <see cref="ApplicationClaimsPrincipalFactory{TApplication}"/>.
        /// </param>
        public LoginContextFactory(
            UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> userClaimsPrincipalFactory,
            ApplicationManager<TApplication> applicationManager,
            IApplicationClaimsPrincipalFactory<TApplication> applicationClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _applicationManager = applicationManager;
            _applicationClaimsPrincipalFactory = applicationClaimsPrincipalFactory;
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> GetApplicationAsync(string clientId)
        {
            var application = await _applicationManager.FindByClientIdAsync(clientId);
            return application == null ?
                new ClaimsPrincipal(new ClaimsIdentity()) :
                await _applicationClaimsPrincipalFactory.CreateAsync(application);
        }

        /// <inheritdoc />
        public async Task<ClaimsPrincipal> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ?
                new ClaimsPrincipal(new ClaimsIdentity()) :
                await _userClaimsPrincipalFactory.CreateAsync(user);
        }
    }
}
