using System;
using System.Web.Mvc;
using Global;
using Secure_Coding.MvcSecurityExtensions;
using Web.Data.Model;
using Web.Data.Contract;
using Web.Data.Repository;

namespace Web.Admin.Controllers
{   
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class ApiCredentialController : RootController
    {
		private readonly IApiCredentialRepository apiCredentialRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public ApiCredentialController() : this(new ApiCredentialRepository())
        {
        }

        public ApiCredentialController(IApiCredentialRepository apiCredentialRepository)
        {
			this.apiCredentialRepository = apiCredentialRepository;
        }

        //
        // GET: /ApiCredential/

        public ViewResult Index()
        {
			var model = apiCredentialRepository.All;
            return View(model);
        }

        //
        // GET: /ApiCredential/Details/5

        public ViewResult Details(int id)
        {
			var model = apiCredentialRepository.Find(id);
            return View(model);
        }

        //
        // GET: /ApiCredential/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ApiCredential/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ApiCredential apiCredential)
        {
            if (ModelState.IsValid)
            {
                apiCredential.IsActive = true;
                apiCredential.Key = Utility.GetCode(true, 8);
                apiCredential.Secret = Utility.GetCode(true, 8);
                apiCredential.DateCreated = DateTime.Now;

                apiCredentialRepository.InsertOrUpdate(apiCredential);
                apiCredentialRepository.Save();

                FlashSuccess("Successfully added API credential.");
                return RedirectToAction("Index");
            }

            FlashValidationError();
            return View(apiCredential);
        }

        //
        // GET: /ApiCredential/Edit/5
 
        public ActionResult Edit(int id)
        {
             var model = apiCredentialRepository.Find(id);
            return View(model);
        }

        //
        // POST: /ApiCredential/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateAntiModelInjection("DateCreated, Key, Secret, ApiCredentialId")]
        public ActionResult Edit(ApiCredential apiCredential)
        {
            if (ModelState.IsValid)
            {
                apiCredentialRepository.InsertOrUpdate(apiCredential);
                apiCredentialRepository.Save();

                FlashSuccess("Successfully updated API credential.");
                return RedirectToAction("Index");
            }

            FlashValidationError();
            return View(apiCredential);
        }

        //
        // GET: /ApiCredential/Delete/5
 
        public ActionResult Delete(int id)
        {
			var model = apiCredentialRepository.Find(id);
            return View(model);
        }

        //
        // POST: /ApiCredential/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            apiCredentialRepository.Delete(id);
            apiCredentialRepository.Save();

			FlashSuccess("Successfully deleted API credential.");
            return RedirectToAction("Index");
        }
    }
}

