using Microsoft.AspNet.Http.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace IdentitySample.Models
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        public SignInManager<ApplicationUser> SignInManager { get; private set; }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var signInStatus = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        ModelState.AddModelError("", "User is locked out, try again later.");
                        return View(model);
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid username or password.");
                        return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            SignInManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            // Request a redirect to the external login provider
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Dictionary[ProviderKey] = provider;
            return new ChallengeResult(provider, properties);//, 
        }

        private const string ProviderKey = "Provider";

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var auth = await Context.AuthenticateAsync(ClaimsIdentityOptions.DefaultExternalLoginAuthenticationType);

            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (auth == null || auth.Properties == null || auth.Properties.Dictionary == null || !auth.Properties.Dictionary.ContainsKey(ProviderKey))
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            // TODO: This used to be GetExternalLoginInfo
            var providerKey = auth.Identity.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = auth.Properties.Dictionary[ProviderKey] as string;
            if (provider == null)
            {
                return RedirectToAction("Login");
            }

            var result = await SignInManager.ExternalLoginSignInAsync(provider, providerKey, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = provider;
                    var email = auth.Identity.FindFirstValue(ClaimTypes.Email);
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email /*loginInfo.Email*/ });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                var auth = await Context.AuthenticateAsync(ClaimsIdentityOptions.DefaultExternalLoginAuthenticationType);

                // Get the information about the user from the external login provider
                //var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                //if (info == null)
                if (auth == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    var providerKey = auth.Identity.FindFirstValue(ClaimTypes.NameIdentifier);
                    var provider = auth.Properties.Dictionary[ProviderKey] as string;
                    // TODO: what should login provider description be?

                    result = await UserManager.AddLoginAsync(user, new UserLoginInfo(provider, providerKey, provider));
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await UserManager.FindByIdAsync(Context.User.Identity.GetUserId());
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}