using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Service;
using Microsoft.AspNetCore.Identity.Service.Session;

namespace Microsoft.AspNetCore.Applications.Authentication
{
    public class LoginContextFactory<TUser, TApplication> : ILoginFactory
        where TUser : class
        where TApplication : class
    {
        private readonly UserManager<TUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<TUser> _userClaimsPrincipalFactory;
        private readonly ApplicationManager<TApplication> _applicationManager;
        private readonly IApplicationClaimsPrincipalFactory<TApplication> _applicationClaimsPrincipalFactory;

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

        public async Task<ClaimsPrincipal> GetApplicationAsync(string clientId)
        {
            var application = await _applicationManager.FindByClientIdAsync(clientId);
            return application == null ?
                new ClaimsPrincipal(new ClaimsIdentity()) :
                await _applicationClaimsPrincipalFactory.CreateAsync(application);
        }

        public async Task<ClaimsPrincipal> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ?
                new ClaimsPrincipal(new ClaimsIdentity()) :
                await _userClaimsPrincipalFactory.CreateAsync(user);
        }
    }
}
