using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Web.Admin.Models;
using Web.Data.Contract;
using Web.Data.Model;
using Web.Data.Repository;

namespace Web.Admin.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminAccountController : RootController
    {
        private readonly IUserAccountRepository userAccountRepository;

        public AdminAccountController() : this(new UserAccountRepository()){}

        public AdminAccountController(IUserAccountRepository userAccountRepository)
        {
            this.userAccountRepository = userAccountRepository;
        }

        public ActionResult Index(int? page)
        {
            var model = userAccountRepository.AllAccounts().Where(x => x.UserName != User.Identity.Name).OrderByDescending(x => x.DateCreated);
            return View(model);
        }

        public ActionResult AddToRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToRole(RoleMgt roleMgt)
        {
            if (ModelState.IsValid)
            {
                var user = userAccountRepository.FindByEmail(roleMgt.Email);
                if (user == null)
                {
                    FlashError("User account was not found");
                    return View(roleMgt);
                }

                if (roleMgt.Role != "Admin" && roleMgt.Role != "SuperAdmin")
                {
                    FlashError("Invalid role selected");
                    return View(roleMgt);
                }

                var context = new ApplicationDbContext();
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                manager.AddToRole(user.Id, roleMgt.Role);

                FlashSuccess($"{roleMgt.Email} has been made {roleMgt.Role}");
                return RedirectToAction("AddToRole");
            }

            FlashValidationError();
            return View(roleMgt);
        }

        public ActionResult RemoveFromRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RemoveFromRole(RoleMgt roleMgt)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.Name == roleMgt.Email)
                {
                    FlashError("You cannot run this operation on your own account");
                    return View(roleMgt);
                }

                var user = userAccountRepository.FindByEmail(roleMgt.Email);
                if (user == null)
                {
                    FlashError("User account was not found");
                    return View(roleMgt);
                }

                if (roleMgt.Role != "Admin" && roleMgt.Role != "SuperAdmin")
                {
                    FlashError("Invalid role selected");
                    return View(roleMgt);
                }

                var context = new ApplicationDbContext();
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                manager.RemoveFromRole(user.Id, roleMgt.Role);

                FlashSuccess($"{roleMgt.Email} has been removed from {roleMgt.Role} role");
                return RedirectToAction("RemoveFromRole");
            }

            FlashValidationError();
            return View(roleMgt);
        }

        public ActionResult CheckRole()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CheckRole(CheckRole checkRole)
        {
            if (ModelState.IsValid)
            {
                var user = userAccountRepository.FindByEmail(checkRole.Email);
                if (user == null)
                {
                    FlashError("User account was not found");
                    return View(checkRole);
                }

                var context = new ApplicationDbContext();
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);

                var roles = manager.GetRoles(user.Id);

                var sb = new StringBuilder();
                foreach (var role in roles)
                {
                    sb.Append($"{role}, ");
                }

                var _roles = sb.ToString().TrimEnd(',');
                var msg = $"{checkRole.Email} has the following roles: {_roles.Trim()}";

                FlashSuccess(msg);
                return RedirectToAction("index", "Home");
            }

            FlashValidationError();
            return View(checkRole);
        }
    }
}