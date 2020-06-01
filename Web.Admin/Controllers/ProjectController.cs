using System;
using System.Web.Mvc;
using Web.Data.Model;
using Web.Data.Contract;
using Web.Data.Repository;

namespace Web.Admin.Controllers
{   
    public class ProjectController : RootController
    {
		private readonly IProjectRepository projectRepository;

		// If you are using Dependency Injection, you can delete the following constructor
        public ProjectController() : this(new ProjectRepository())
        {
        }

        public ProjectController(IProjectRepository projectRepository)
        {
			this.projectRepository = projectRepository;
        }

        //
        // GET: /Project/

        public ViewResult Index()
        {
			var model = projectRepository.AllIncluding(project => project.ProjectDataSets);
            return View(model);
        }

        //
        // GET: /Project/Details/5

        public ViewResult Details(int id)
        {
			var model = projectRepository.Find(id);
            return View(model);
        }

        //
        // GET: /Project/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Project/Create

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Create(Project project)
        {
            if (ModelState.IsValid) {

                project.DateCreated = DateTime.Now;
                project.IsActive = true;
                projectRepository.InsertOrUpdate(project);
                projectRepository.Save();
				
				FlashSuccess("Successfully added project.");
                return RedirectToAction("Index");
            } 
			
				FlashValidationError();
				return View(project);
        }
        
        //
        // GET: /Project/Edit/5
 
        public ActionResult Edit(int id)
        {
             var model = projectRepository.Find(id);
            return View(model);
        }

        //
        // POST: /Project/Edit/5

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Edit(Project project)
        {
            if (ModelState.IsValid) {
                projectRepository.InsertOrUpdate(project);
                projectRepository.Save();

				FlashSuccess("Successfully updated project.");
                return RedirectToAction("Index");
            } 
			
				FlashValidationError();
				return View(project);
        }

        //
        // GET: /Project/Delete/5
 
        public ActionResult Delete(int id)
        {
			var model = projectRepository.Find(id);
            return View(model);
        }

        //
        // POST: /Project/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            projectRepository.Delete(id);
            projectRepository.Save();

			FlashSuccess("Successfully deleted project.");
            return RedirectToAction("Index");
        }
    }
}

