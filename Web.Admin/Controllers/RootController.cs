using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Security;

namespace Web.Admin.Controllers
{
    public class RootController : Controller
    {
        protected List<string> errors = new List<string>();

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BaseController" /> class.
        /// </summary>
        /// <param name = "userManager">The membership provider.</param>
        private RootController(MembershipProvider userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "BaseController" /> class.
        /// </summary>
        protected RootController()
            : this(Membership.Provider) { }

        protected bool IsRoot(string username)
        {
            return username == "root";
        }

        protected bool IsAdmin()
        {
            return User.IsInRole("Administrators");
        }

        #region Flash - Validation

        protected void FlashValidationError()
        {
            Flash("", "warning");
        }

        protected void FlashWarning(string msg)
        {
            Flash(msg, "warning");
        }

        protected void FlashError(string msg)
        {
            Flash(msg, "error");
        }

        protected void FlashInfo(string msg)
        {
            Flash(msg, "info");
        }

        protected void FlashSuccess(string msg)
        {
            Flash(msg, "success");
        }

        private void Flash(string message, string category)
        {
            TempData["flash"] = message;
            TempData["flashmode"] = category;
        }

        protected void SetInvalid(string key)
        {
            TempData["Error:" + key] = Request.Form[key];
        }

        protected void SetValidationErrors(IDictionary errors)
        {
            IEnumerator it = errors.Keys.GetEnumerator();
            while (it.MoveNext()) SetInvalid(it.Current.ToString().Split(' ')[1]);
        }

        #endregion Flash - Validation

        #region Membership

        private readonly MembershipProvider _userManager;

        /// <summary>
        ///   Gets the MembershipProvider.
        /// </summary>
        /// <value>The user manager.</value>
        protected MembershipProvider UserManager
        {
            [DebuggerStepThrough]
            get { return _userManager; }
        }

        /// <summary>
        ///   Gets a value indicating whether the currently visiting user is authenticated.
        /// </summary>
        /// <value>
        ///   if user authenticated returns <c>true</c>; otherwise, <c>false</c>.
        /// </value>
        protected bool IsUserAuthenticated
        {
            [DebuggerStepThrough]
            get { return HttpContext.User.Identity.IsAuthenticated; }
        }

        /// <summary>
        ///   Gets the name of the current user. If the user is not authenticated it returns "Anonymous"
        /// </summary>
        /// <value>The name of the current user.</value>
        protected string CurrentUserName
        {
            [DebuggerStepThrough]
            get { return IsUserAuthenticated ? HttpContext.User.Identity.Name : "Anonymous"; }
        }

        /// <summary>
        ///   Gets the current user id. if the user is not authenticated it returns empty guid.
        /// </summary>
        /// <value>The current user id.</value>
        protected Guid CurrentUserId
        {
            [DebuggerStepThrough]
            get
            {
                if (!IsUserAuthenticated) return Guid.Empty;

                MembershipUser user = UserManager.GetUser(CurrentUserName, true);

                return (Guid)user.ProviderUserKey;
            }
        }

        #endregion Membership
    }
}