using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Global;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Web.Admin.Models;
using Web.Data.Model;

namespace Web.Admin.Controllers
{
    [Authorize]
    public class ManageController : RootController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>(); }
            private set { _signInManager = value; }
        }

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        // GET: /Manage/Index
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByEmailAsync(User.Identity.Name);

            var model = new AccountViewModel
            {
                Username = user.UserName,
                AccountName = user.Name,
                Phone = user.PhoneNumber
            };

            return View(model);
        }

        public async Task<ActionResult> ChangeAccountName()
        {
            var user = await UserManager.FindByEmailAsync(User.Identity.Name);

            var model = new ChangeAccountName
            {
                AccountName = user.Name,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeAccountName(ChangeAccountName model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(User.Identity.Name);

                try
                {
                    user.Name = model.AccountName;
                }
                catch (ApplicationException ex)
                {
                    FlashError(ex.Message);
                    return View(model);
                }

                await UserManager.UpdateAsync(user);

                FlashSuccess("Account name updated successfully");
                return RedirectToAction("index");
            }

            FlashValidationError();
            return View(model);
        }

        public async Task<ActionResult> ChangeAccountPhone()
        {
            var user = await UserManager.FindByEmailAsync(User.Identity.Name);

            var model = new ChangeAccountPhone
            {
                Phone = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAccountPhone(ChangeAccountPhone model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserManager.FindByEmail(User.Identity.Name);

                    user.PhoneNumber = model.Phone;
                    UserManager.Update(user);

                    FlashSuccess("Phone number updated successfully.");
                    return RedirectToAction("Index");
                }
                catch (ApplicationException ex)
                {
                    FlashError(ex.Message);
                    return View(model);
                }
            }

            FlashValidationError();
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                FlashValidationError();
                return View(model);
            }
            var result =
                await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }

                FlashSuccess(ManageMessageId.ChangePasswordSuccess.ToString());
                return RedirectToAction("Index");
            }
            AddErrors(result);
            return View(model);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie,
                DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent },
                await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion Helpers
    }
}