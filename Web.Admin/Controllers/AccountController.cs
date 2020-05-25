using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Data.Contract;
using Web.Data.Model;
using Web.Data.Repository;

namespace Web.Admin.Controllers
{
    public class AccountController : RootController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly IUserAccountRepository userAccountRepository = new UserAccountRepository();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                FlashValidationError();
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            switch (result)
            {
                case SignInStatus.Success:
                    userAccountRepository.UpdateLoginDate(model.Email);
                    return RedirectToLocal(returnUrl);

                default:
                    FlashError("Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Details(string id)
        {
            var model = userAccountRepository.FindByUserId(id);

            return View(new UserAccount
            {
                UserName = model.UserName,
                Id = model.Id,
                Name = model.Name,
                LastLoginDate = model.LastLoginDate,
                DateCreated = model.DateCreated
            });
        }

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FlashValidationError();
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                Name = model.Name,
                DateCreated = DateTime.Now,
                LastLoginDate = new DateTime(1990, 01, 01)
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {

                await UserManager.AddToRoleAsync(user.Id, model.AccessLevel);

                FlashSuccess("Successfully added user");
                return RedirectToAction("Index", "AdminAccount");
            }

            FlashError($"{model.Name} already has a user account on the system.");
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> ResetPassword(string id)
        {
            var code = await UserManager.GeneratePasswordResetTokenAsync(id);
            var result = await UserManager.ResetPasswordAsync(id, code, "password");

            if (result.Succeeded)
            {
                FlashSuccess("Password has been reset successfully");
                return RedirectToAction("index", "AdminAccount");
            }

            FlashError("An error occurred.");
            return RedirectToAction("index", "AdminAccount");
        }

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(string id)
        {
            var model = userAccountRepository.FindByUserId(id);

            return View(new UserAccount
            {
                UserName = model.UserName,
                Id = model.Id,
                Name = model.Name,
                LastLoginDate = model.LastLoginDate,
                DateCreated = model.DateCreated
            });
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            var result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                FlashSuccess("User deleted successfully");
                return RedirectToAction("index", "AdminAccount");
            }

            FlashError("An error occurred.");
            return RedirectToAction("index", "AdminAccount");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion Helpers
    }
}